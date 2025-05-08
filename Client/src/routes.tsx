import {Route, Routes} from "react-router";
import LoginPage from "@/pages/public/LoginPage";
import { AuthenticatedLayout } from "@/components/layouts/AuthenticatedLayout";
import DashboardPage from "@/pages/auth/DashboardPage";
import DataPage from "@/pages/auth/DataPage";
import useSubscribeToTopics from "@/hooks/use-subscripte-to-topic";
import { ROUTE } from "@/routes-constants";

export default function ApplicationRoutes() {
  useSubscribeToTopics();

  return (
    <Routes>
      <Route path="/" element={<LoginPage />} />

      <Route element={<AuthenticatedLayout />}>
        <Route path={ROUTE.DASHBOARD.INDEX} element={<DashboardPage />} />
        <Route path={ROUTE.DASHBOARD.DATA} element={<DataPage />} />
      </Route>

    </Routes>
  )
}