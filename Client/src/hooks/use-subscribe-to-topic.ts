import {useEffect} from "react";
import {ChangeSubscriptionDto, StringConstants, WebsocketTopics} from "../generated-client.ts";
import {useWsClient} from "ws-request-hook";
import { subscriptionClient } from "@/api/api-controller-clients.ts";
import { randomUid } from "@/App.tsx";
import { toast } from "sonner";

export default function useSubscribeToTopics() {

    const [jwt] = "asdasdasd";
    const {readyState} = useWsClient();

    useEffect(() => {
        if (readyState != 1 || jwt == null || jwt.length < 1)
            return;
        const subscribeDto: ChangeSubscriptionDto = {
            clientId: randomUid,
            topicIds: ["measurements"],
        };

        subscriptionClient.subscribe(jwt, subscribeDto).then(r => {
        toast.success("Subscription", {
          description: "Successfully subscribing to " + WebsocketTopics.Dashboard,
          action: {
            label: "OK",
            onClick: () => {}, // lidt dumt, men ellers kan man ik trykke ok
          }
        })
        })

    }, [readyState, jwt])
}