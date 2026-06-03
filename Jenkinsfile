// Jenkins pipeline - build on Jenkins (LAN) and deploy to a remote server.
//
// Architecture:
//   Jenkins agent (LAN, has .NET 10 SDK + docker CLI)
//     |
//     |  dotnet build/test/publish  (local)
//     |  docker build               (local)
//     |
//     |  docker save <image> | ssh user@remote "docker load && ..."
//     v
//   Remote server (docker engine, the actual production host)
//
// Image transfer uses a streamed `docker save | ssh ... docker load` pipe
// so no tar file is written to disk on either side. No registry required.
//
// Jenkins prerequisites (configure once on the controller):
//   - Credentials: an "SSH Username with private key" entry, e.g. id = "remote-ssh-key"
//     whose username matches the SSH user on the remote (commonly "deploy" or "root").
//   - Plugins:  ssh-agent
//
// Remote prerequisites:
//   - docker CLI on PATH for the SSH user
//   - env files present (managed out-of-band, NOT in this repo):
//       <REMOTE_API_ENV_FILE>        - API secrets (ConnectionStrings, AuthServer, ...)
//       <REMOTE_MIGRATOR_ENV_FILE>   - migrator secrets (ConnectionStrings, ...)
//     Populate from rotated appsettings.secrets.json values.
//
// Trigger: configure a Jenkins job pointing at this file (Pipeline from SCM).
// Branch filtering: handle in the job config (e.g. only build master/main).

pipeline {
    agent any

    options {
        timestamps()
        buildDiscarder(logRotator(numToKeepStr: '20', artifactNumToKeepStr: '5'))
        disableConcurrentBuilds()
        timeout(time: 30, unit: 'MINUTES')
    }

    parameters {
        // ---- build ----
        booleanParam(name: 'RUN_TESTS',         defaultValue: true, description: 'Run dotnet test before publish')
        booleanParam(name: 'BUILD_DB_MIGRATOR', defaultValue: true, description: 'Also build the DbMigrator image')

        // ---- deploy target ----
        booleanParam(name: 'RUN_DB_MIGRATOR',   defaultValue: true, description: 'Run the DbMigrator container on the remote before deploying the API')
        booleanParam(name: 'DEPLOY_API',        defaultValue: true, description: 'Rolling-restart the API container on the remote')

        // ---- remote host ----
        string(name: 'REMOTE_HOST',              defaultValue: '',                   description: 'Remote server hostname or IP (required when DEPLOY_API or RUN_DB_MIGRATOR is true)')
        string(name: 'REMOTE_SSH_CREDENTIALS',   defaultValue: 'remote-ssh-key',     description: 'Jenkins SSH credential id (SSH Username with private key) for the remote user')
        string(name: 'REMOTE_API_PORT',          defaultValue: '8080',               description: 'Port published on the remote host (mapped to container port 80)')

        // ---- container / env files on the remote ----
        string(name: 'API_CONTAINER_NAME',       defaultValue: 'luckylotapi-api',           description: 'Docker container name for the API on the remote')
        string(name: 'MIGRATOR_CONTAINER_NAME',  defaultValue: 'luckylotapi-migrator',      description: 'Docker container name for the migrator (one-shot) on the remote')
        string(name: 'REMOTE_API_ENV_FILE',      defaultValue: '/etc/luckylotapi/luckylotapi.env',          description: 'Path on the remote to the API env-file (secrets)')
        string(name: 'REMOTE_MIGRATOR_ENV_FILE', defaultValue: '/etc/luckylotapi/luckylotapi-migrator.env', description: 'Path on the remote to the migrator env-file (secrets)')
    }

    environment {
        SOLUTION              = 'Ydls.LuckyLotApi.slnx'
        HTTPAPI_PROJECT       = 'src/Ydls.LuckyLotApi.HttpApi.Host/Ydls.LuckyLotApi.HttpApi.Host.csproj'
        HTTPAPI_DOCKERFILE    = 'src/Ydls.LuckyLotApi.HttpApi.Host/Dockerfile'
        HTTPAPI_IMAGE         = 'luckylotapi-api'
        DBMIGRATOR_PROJECT    = 'src/Ydls.LuckyLotApi.DbMigrator/Ydls.LuckyLotApi.DbMigrator.csproj'
        DBMIGRATOR_DOCKERFILE = 'src/Ydls.LuckyLotApi.DbMigrator/Dockerfile'
        DBMIGRATOR_IMAGE      = 'luckylotapi-migrator'
        IMAGE_TAG             = "${BUILD_NUMBER}-${env.GIT_COMMIT?.take(7) ?: 'local'}"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
                sh 'git rev-parse --short HEAD > .git/short-commit'
                script {
                    env.GIT_SHORT = readFile('.git/short-commit').trim()
                    env.IMAGE_TAG = "${BUILD_NUMBER}-${env.GIT_SHORT}"
                }
            }
        }

        stage('Restore & Build') {
            steps {
                sh "dotnet restore ${SOLUTION}"
                sh "dotnet build ${SOLUTION} -c Release --no-restore"
            }
        }

        stage('Test') {
            when { expression { return params.RUN_TESTS } }
            steps {
                sh "dotnet test ${SOLUTION} -c Release --no-build --no-restore --logger 'trx;LogFileName=test-results.trx'"
            }
            post {
                always {
                    testResults '**/TestResults/*.trx'
                }
            }
        }

        stage('Publish HttpApi.Host') {
            steps {
                sh "dotnet publish ${HTTPAPI_PROJECT} -c Release -o ${env.WORKSPACE}/src/Ydls.LuckyLotApi.HttpApi.Host/bin/Release/net10.0/publish --no-restore --no-build"
            }
        }

        stage('Build HttpApi.Host image') {
            steps {
                sh "docker build -f ${HTTPAPI_DOCKERFILE} -t ${HTTPAPI_IMAGE}:${IMAGE_TAG} -t ${HTTPAPI_IMAGE}:latest src/Ydls.LuckyLotApi.HttpApi.Host"
            }
        }

        stage('Publish DbMigrator') {
            when { expression { return params.BUILD_DB_MIGRATOR } }
            steps {
                sh "dotnet publish ${DBMIGRATOR_PROJECT} -c Release -o ${env.WORKSPACE}/src/Ydls.LuckyLotApi.DbMigrator/bin/Release/net10.0/publish --no-restore --no-build"
            }
        }

        stage('Build DbMigrator image') {
            when { expression { return params.BUILD_DB_MIGRATOR } }
            steps {
                sh "docker build -f ${DBMIGRATOR_DOCKERFILE} -t ${DBMIGRATOR_IMAGE}:${IMAGE_TAG} -t ${DBMIGRATOR_IMAGE}:latest src/Ydls.LuckyLotApi.DbMigrator"
            }
        }

        stage('Transfer & Run DbMigrator on Remote') {
            when { expression { return params.BUILD_DB_MIGRATOR && params.RUN_DB_MIGRATOR } }
            steps {
                sshagent(credentials: [params.REMOTE_SSH_CREDENTIALS]) {
                    sh """
                        set -e
                        test -n "${params.REMOTE_HOST}" || { echo 'REMOTE_HOST is required' >&2; exit 1; }
                        docker save ${DBMIGRATOR_IMAGE}:${IMAGE_TAG} \
                            | ssh -o StrictHostKeyChecking=accept-new -o ServerAliveInterval=30 ${params.REMOTE_HOST} "
                                set -e
                                test -f ${params.REMOTE_MIGRATOR_ENV_FILE} \\
                                    || { echo 'Missing remote env file: ${params.REMOTE_MIGRATOR_ENV_FILE}' >&2; exit 1; }
                                docker load
                                docker run --rm \\
                                    --name ${params.MIGRATOR_CONTAINER_NAME} \\
                                    --env-file ${params.REMOTE_MIGRATOR_ENV_FILE} \\
                                    ${DBMIGRATOR_IMAGE}:${IMAGE_TAG}
                            "
                    """
                }
            }
        }

        stage('Transfer & Deploy HttpApi.Host to Remote') {
            when { expression { return params.DEPLOY_API } }
            steps {
                sshagent(credentials: [params.REMOTE_SSH_CREDENTIALS]) {
                    sh """
                        set -e
                        test -n "${params.REMOTE_HOST}" || { echo 'REMOTE_HOST is required' >&2; exit 1; }
                        docker save ${HTTPAPI_IMAGE}:${IMAGE_TAG} \
                            | ssh -o StrictHostKeyChecking=accept-new -o ServerAliveInterval=30 ${params.REMOTE_HOST} "
                                set -e
                                test -f ${params.REMOTE_API_ENV_FILE} \\
                                    || { echo 'Missing remote env file: ${params.REMOTE_API_ENV_FILE}' >&2; exit 1; }
                                docker stop ${params.API_CONTAINER_NAME} || true
                                docker rm   ${params.API_CONTAINER_NAME} || true
                                docker load
                                docker run -d \\
                                    --name ${params.API_CONTAINER_NAME} \\
                                    --restart unless-stopped \\
                                    -p ${params.REMOTE_API_PORT}:80 \\
                                    --env-file ${params.REMOTE_API_ENV_FILE} \\
                                    ${HTTPAPI_IMAGE}:${IMAGE_TAG}
                            "
                    """
                }
            }
        }

        stage('Health Check') {
            when { expression { return params.DEPLOY_API } }
            steps {
                sshagent(credentials: [params.REMOTE_SSH_CREDENTIALS]) {
                    sh """
                        # Wait for the container to come up, then probe /health-status.
                        for i in 1 2 3 4 5 6 7 8 9 10; do
                            sleep 3
                            code=\$(curl -s -o /dev/null -w '%{http_code}' \\
                                http://${params.REMOTE_HOST}:${params.REMOTE_API_PORT}/health-status || true)
                            if [ "\$code" = "200" ]; then
                                echo "Health check OK (attempt \$i)"
                                exit 0
                            fi
                            echo "Health check attempt \$i returned \$code, retrying..."
                        done
                        echo 'Health check failed after 30s' >&2
                        exit 1
                    """
                }
            }
        }
    }

    post {
        success {
            echo "Built and deployed ${HTTPAPI_IMAGE}:${IMAGE_TAG} to ${params.REMOTE_HOST}:${params.REMOTE_API_PORT} (container=${params.API_CONTAINER_NAME})."
        }
        failure {
            echo 'Build or deploy failed - check the logs above.'
        }
        always {
            sh 'rm -f .git/short-commit || true'
        }
    }
}
