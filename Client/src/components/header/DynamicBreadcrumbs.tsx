import { useState, useEffect, Fragment } from "react";
import { useLocation } from "react-router-dom";
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbSeparator } from "../ui/breadcrumb";

const capitalizeString = (str: string) => {
  return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
};

const DynamicBreadcrumbs = () => {
  const location = useLocation();
  const [breadcrumbs, setBreadcrumbs] = useState<{ label: string, href: string }[]>([]);

  useEffect(() => {
    const pathSegments = location.pathname.split("/").filter(Boolean);

    const breadcrumbList = pathSegments.map((segment, index, array) => {
      const href = `/${array.slice(0, index + 1).join("/")}`;
      
      const label = capitalizeString(segment.replace(/-/g, " "));
      return { label, href };
    });

    setBreadcrumbs(breadcrumbList);
  }, [location.pathname]);

  return (
    <Breadcrumb>
      <BreadcrumbList>
        {breadcrumbs.map((breadcrumb, index) => (
          <Fragment key={index}>
            <BreadcrumbItem>
              <BreadcrumbLink href={breadcrumb.href}>
                {breadcrumb.label}
              </BreadcrumbLink>
            </BreadcrumbItem>
            {index < breadcrumbs.length - 1 && <BreadcrumbSeparator />}
          </Fragment>
        ))}
      </BreadcrumbList>
    </Breadcrumb>
  );
};

export default DynamicBreadcrumbs;
