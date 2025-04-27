import * as React from "react"
import {
  Book,
  Database,
  DownloadIcon,
  EthernetPort,
  LifeBuoy,
  Send,
  SquareTerminal,
  Tv2,
  Wrench,
} from "lucide-react"

import { NavMain } from "./NavMain"
import { NavUser } from "./NavUser"
import { NavHeader } from "./NavHeader"
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarRail,
} from "@/components/ui/sidebar"
import { NavTop } from "./NavTop"
import { ROUTE } from "@/routes"

const data = {
  user: {
    name: "shadcn",
    email: "m@example.com",
    avatar: "/avatars/shadcn.jpg",
  },
  top: [
    {
      name: "Dashboard",
      url: ROUTE.DASHBOARD.INDEX,
      icon: Tv2,
    },
    {
        name: "Data",
        url: ROUTE.DASHBOARD.DATA,
        icon: Tv2,
      },
      {
        name: "AI",
        url: ROUTE.DASHBOARD.AI,
        icon: Tv2,
      },
  ],
  navMain: [
    {
      title: "Device",
      url: ROUTE.DASHBOARD.INDEX,
      icon: Wrench,
      isActive: true,
      items: [
        {
          title: "Panel",
          url: ROUTE.DASHBOARD.DEVICE.INDEX,
        },
        {
          title: "Data",
          url: ROUTE.DASHBOARD.DEVICE.DATA,
        },
        {
          title: "Configuration",
          url: ROUTE.DASHBOARD.DEVICE.CONFIG,
        },
      ],
    },
  ],
}

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  return (
    <Sidebar collapsible="icon" {...props}>
      <SidebarHeader>
        <NavHeader/>
      </SidebarHeader>
      <SidebarContent>
        <NavTop items={data.top} />
        <NavMain items={data.navMain} />
      </SidebarContent>
      <SidebarFooter className="border-t">
        <NavUser user={data.user} />
      </SidebarFooter>
      <SidebarRail />
    </Sidebar>
  )
}