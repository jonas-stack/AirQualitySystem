import {useEffect} from "react";
import {ChangeSubscriptionDto, StringConstants, WebsocketTopics} from "../generated-client.ts";
import {useWsClient} from "ws-request-hook";
import { subscriptionClient } from "@/api/api-controller-clients.ts";
import { randomUid } from "@/App.tsx";

export default function useSubscribeToTopics() {

    const [jwt] = "asdasdasd";
    const {readyState} = useWsClient();

    useEffect(() => {
        if (readyState != 1 || jwt == null || jwt.length < 1)
            return;
        const subscribeDto: ChangeSubscriptionDto = {
            clientId: randomUid,
            topicIds: [WebsocketTopics.Dashboard],
        };

        //TODO: Vis en form for modal, det har shadcn indbygget.
        subscriptionClient.subscribe(jwt, subscribeDto).then(r => {
            console.log("you are subscribed")
        })

    }, [readyState, jwt])
}