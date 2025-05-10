import { useEffect, useState } from 'react';
import ApplicationRoutes from '@/routes';
import { DevTools } from 'jotai-devtools';
import { WsClientProvider } from 'ws-request-hook';
import 'jotai-devtools/styles.css';

const baseUrl = import.meta.env.VITE_API_BASE_URL
const prod = import.meta.env.PROD

export const randomUid = crypto.randomUUID();

export default function App() {
  const [serverUrl, setServerUrl] = useState<string | undefined>(undefined)
    
  useEffect(() => {
      const finalUrl = prod ? 'wss://' + baseUrl + '?id=' + randomUid : 'ws://' + baseUrl + '?id=' + randomUid;
      setServerUrl(finalUrl);
      console.log(finalUrl)
  }, [prod, baseUrl]);

  return (<>
      {serverUrl && (
          <WsClientProvider url={serverUrl} >
              <ApplicationRoutes />
          </WsClientProvider>
      )}
      {!serverUrl && <DevTools />}
      </>);
}
