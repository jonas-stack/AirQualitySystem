import {useEffect} from "react";
import {ChangeSubscriptionDto, StringConstants} from "../generated-client.ts";
import {useWsClient} from "ws-request-hook";
import { randomUid } from "@/App.tsx";
import { subscriptionClient } from "@/api/api-controller-clients.ts";

export default function useSubscribeToTopics() {
    const {readyState} = useWsClient();

    // tom for nu, vi har ikke implementeret auth
    const jwt = "jwtKey"; 

    useEffect(() => {
        console.log("WebSocket readyState:", readyState);

        if (readyState != 1 || jwt == null || jwt.length < 1)
            return;

        console.log("ready")
        const subscribeDto: ChangeSubscriptionDto = {
            clientId: randomUid,
            topicIds: [StringConstants.ExampleClientDto],
        };
        console.log(subscriptionClient)
        subscriptionClient.subscribe(jwt, subscribeDto)
        .then(r => {
          console.log("subscribed");
        })
        .catch((error) => {
          console.error("Subscription failed:", error);
        });
    }, [readyState, jwt])
}