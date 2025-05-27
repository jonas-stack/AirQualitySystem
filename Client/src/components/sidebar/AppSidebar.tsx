import * as React from "react"
import {
  Bot,
  ChartArea,
  Cpu,
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
      name: "Device",
      url: ROUTE.DASHBOARD.DEVICE.INDEX,
      icon: Cpu,
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
        <NavTop title="Home" items={data.top} />
        <NavTop title="Platform" items={data.navMain} />
      </SidebarContent>
      <SidebarRail />
    </Sidebar>
  )
}