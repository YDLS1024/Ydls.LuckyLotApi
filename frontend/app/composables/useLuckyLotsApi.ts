import type {
  CreateExpertsDto,
  CreateKillNumbersDto,
  CreateNumberThreeDto,
  ExpertsDto,
  KillNumbersDto,
  NumberThreeDto,
  PagedResultDto,
  UpdateExpertsDto,
  UpdateKillNumbersDto,
  UpdateNumberThreeDto
} from '~/types/luckyLots'

function toQuery(params: Record<string, string | number | undefined | null>) {
  const search = new URLSearchParams()
  for (const [key, value] of Object.entries(params)) {
    if (value !== undefined && value !== null && value !== '') {
      search.set(key, String(value))
    }
  }
  const query = search.toString()
  return query ? `?${query}` : ''
}

export function useLuckyLotsApi() {
  const { request } = useApiClient()

  const numberThree = {
    list: (skipCount = 0, maxResultCount = 20, sorting = 'OpenDate DESC') =>
      request<PagedResultDto<NumberThreeDto>>(
        `/api/app/number-three${toQuery({ skipCount, maxResultCount, sorting })}`,
        { auth: false }
      ),
    get: (id: string) =>
      request<NumberThreeDto>(`/api/app/number-three/${id}`, { auth: false }),
    create: (input: CreateNumberThreeDto) =>
      request<NumberThreeDto>('/api/app/number-three', {
        method: 'POST',
        body: JSON.stringify(input),
        auth: true
      }),
    update: (id: string, input: UpdateNumberThreeDto) =>
      request<NumberThreeDto>(`/api/app/number-three/${id}`, {
        method: 'PUT',
        body: JSON.stringify(input),
        auth: true
      }),
    remove: (id: string) =>
      request<void>(`/api/app/number-three/${id}`, { method: 'DELETE', auth: true })
  }

  const experts = {
    list: (skipCount = 0, maxResultCount = 50, filter?: string) =>
      request<PagedResultDto<ExpertsDto>>(
        `/api/app/experts${toQuery({ skipCount, maxResultCount, filter })}`,
        { auth: false }
      ),
    get: (id: string) =>
      request<ExpertsDto>(`/api/app/experts/${id}`, { auth: false }),
    create: (input: CreateExpertsDto) =>
      request<ExpertsDto>('/api/app/experts', {
        method: 'POST',
        body: JSON.stringify(input),
        auth: true
      }),
    update: (id: string, input: UpdateExpertsDto) =>
      request<ExpertsDto>(`/api/app/experts/${id}`, {
        method: 'PUT',
        body: JSON.stringify(input),
        auth: true
      }),
    remove: (id: string) =>
      request<void>(`/api/app/experts/${id}`, { method: 'DELETE', auth: true })
  }

  const killNumbers = {
    list: (
      skipCount = 0,
      maxResultCount = 50,
      expertId?: string,
      sorting = 'KillDate DESC'
    ) =>
      request<PagedResultDto<KillNumbersDto>>(
        `/api/app/kill-numbers${toQuery({ skipCount, maxResultCount, expertId, sorting })}`,
        { auth: false }
      ),
    get: (id: string) =>
      request<KillNumbersDto>(`/api/app/kill-numbers/${id}`, { auth: false }),
    create: (input: CreateKillNumbersDto) =>
      request<KillNumbersDto>('/api/app/kill-numbers', {
        method: 'POST',
        body: JSON.stringify(input),
        auth: true
      }),
    update: (id: string, input: UpdateKillNumbersDto) =>
      request<KillNumbersDto>(`/api/app/kill-numbers/${id}`, {
        method: 'PUT',
        body: JSON.stringify(input),
        auth: true
      }),
    remove: (id: string) =>
      request<void>(`/api/app/kill-numbers/${id}`, { method: 'DELETE', auth: true })
  }

  return { numberThree, experts, killNumbers }
}
