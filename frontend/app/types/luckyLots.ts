export interface PagedResultDto<T> {
  totalCount: number
  items: T[]
}

export interface NumberThreeDto {
  id: string
  openDate: string
  one: number
  two: number
  three: number
  creationTime?: string
}

export interface CreateNumberThreeDto {
  openDate: string
  one: number
  two: number
  three: number
}

export interface UpdateNumberThreeDto extends CreateNumberThreeDto {}

export interface ExpertsDto {
  id: string
  nickname: string
  winningRate?: number | null
  killCount: number
  hitCount: number
}

export interface CreateExpertsDto {
  nickname: string
}

export interface UpdateExpertsDto extends CreateExpertsDto {}

export interface KillNumbersDto {
  id: string
  killDate: string
  killNumber: number[]
  isTrue?: boolean | null
  expertId: string
  expertNickname?: string | null
}

export interface CreateKillNumbersDto {
  killDate: string
  killNumber: number[]
  expertId: string
}

export interface UpdateKillNumbersDto extends CreateKillNumbersDto {}

export interface TokenResponse {
  access_token: string
  refresh_token?: string
  expires_in: number
  token_type: string
}
