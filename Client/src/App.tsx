import ApplicationRoutes from '@/routes';
import { ThemeProvider } from '@/context/ThemeContext';
import {DevTools} from "jotai-devtools";
import { useEffect, useState } from 'react';
import { WsClientProvider } from 'ws-request-hook';

const baseUrl = import.meta.env.VITE_API_BASE_URL
const prod = import.meta.env.PROD

export const randomUid = crypto.randomUUID()

export default function App() {
  const [serverUrl, setServerUrl] = useState<string | undefined>(undefined)
    
  useEffect(() => {
      const finalUrl = prod ? 'wss://' + baseUrl + '?id=' + randomUid : 'ws://' + baseUrl + '?id=' + randomUid;
      setServerUrl(finalUrl);
  }, [prod, baseUrl]);

  return (
      <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
         {serverUrl && <WsClientProvider url={serverUrl}>
            <ApplicationRoutes />
         </WsClientProvider>}
         {!prod && <DevTools/>}
      </ThemeProvider>
  )
}
