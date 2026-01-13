import type { Configuration, PopupRequest, SilentRequest } from '@azure/msal-browser'

export function createMsalConfig(
  clientId: string,
  authority: string,
  redirectUri: string
): Configuration {
  return {
    auth: {
      clientId,
      authority,
      redirectUri,
      postLogoutRedirectUri: redirectUri,
      navigateToLoginRequestUrl: true,
    },
    cache: {
      cacheLocation: 'localStorage',
      storeAuthStateInCookie: false,
    },
    system: {
      loggerOptions: {
        logLevel: 3, // Info
        loggerCallback: (level, message, containsPii) => {
          if (containsPii) return
          if (level === 0) console.error(message)
          else if (level === 1) console.warn(message)
          else if (level === 2) console.info(message)
          else console.debug(message)
        },
      },
    },
  }
}

export function createLoginRequest(scopes: string[]): PopupRequest {
  return {
    scopes,
    prompt: 'select_account',
  }
}

export function createSilentRequest(scopes: string[], account: any): SilentRequest {
  return {
    scopes,
    account,
    forceRefresh: false,
  }
}
