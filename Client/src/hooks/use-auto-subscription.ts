import { useEffect } from "react";
import { useWsClient } from "ws-request-hook";
import { useSubscriptionHook } from "./use-subscribe-to-topic";

export function useAutoSubscription(topicIds: string[]) {
  const { readyState } = useWsClient();
  const { subscribe, unsubscribe } = useSubscriptionHook();

  useEffect(() => {
    if (readyState === 1) {
      subscribe(topicIds);
    }

    // unsubscribe når komponenet bliver unmount
  }, [readyState, subscribe, unsubscribe, topicIds]);
}
