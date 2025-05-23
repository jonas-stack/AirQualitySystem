import {useCallback, useState} from "react";
import {ChangeSubscriptionDto, StringConstants, WebsocketTopics} from "../generated-client.ts";
import { subscriptionClient } from "@/api/api-controller-clients.ts";
import { toast } from "sonner";
import { randomUid } from "@/App.tsx";

export function useSubscriptionHook() {
    const clientId = randomUid;
    const jwt = "totallysecretjwt";

  const [isSubscribed, setIsSubscribed] = useState(false);

  // vi bruger usecallback for at sikre der ikke kommer unødvendige re-renders
    const subscribe = useCallback(async (topicIds: string[]) => {
    if (!jwt || jwt.length < 1) return;

    const dto: ChangeSubscriptionDto = {
      clientId,
      topicIds,
    };

    try {
      await subscriptionClient.subscribe(jwt, dto);
      setIsSubscribed(true);
      toast.success("Subscription", {
        description: "Successfully subscribed to " + topicIds,
        action: { label: "OK", onClick: () => {} },
      });
    } catch (error) {
      toast.error("Subscription Failed", {
        description: "Could not subscribe to topics.",
      });
    }
  }, [jwt, clientId]);

  const unsubscribe = useCallback(async (topicIds: string[]) => {
    if (!jwt || jwt.length < 1) return;

    const dto: ChangeSubscriptionDto = {
      clientId,
      topicIds,
    };

    try {
      await subscriptionClient.unsubscribe(jwt, dto);
      setIsSubscribed(false);
      toast.success("Unsubscribed", {
        description: "Successfully unsubscribed from " + topicIds,
        action: { label: "OK", onClick: () => {} },
      });
    } catch (error) {
      toast.error("Unsubscription Failed", {
        description: "Could not unsubscribe from topics.",
      });
    }
  }, [jwt, clientId]);

  return {
    subscribe,
    unsubscribe,
    isSubscribed,
  };
}
