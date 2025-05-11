import * as React from "react"
import {
  Bot,
  ChartArea,
  LayoutGrid,
  Wrench,
} from "lucide-react"

import { NavMain } from "./NavMain"
import { NavHeader } from "./NavHeader"
import {
  Sidebar,
  SidebarContent,
  SidebarHeader,
  SidebarRail,
} from "@/components/ui/sidebar"
import { NavTop } from "./NavTop"
import { ROUTE } from "@/routes-constants"

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
      icon: LayoutGrid,
    },
    {
        name: "Data",
        url: ROUTE.DASHBOARD.DATA,
        icon: ChartArea,
      },
      {
        name: "AI",
        url: ROUTE.DASHBOARD.AI,
        icon: Bot,
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
      <SidebarRail />
    </Sidebar>
  )
}