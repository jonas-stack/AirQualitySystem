import { LoginForm } from "@/components/form/LoginForm";
import Flex from "@/components/utils/Flex";
import Grid from "@/components/utils/Grid";
import { GalleryVerticalEnd } from "lucide-react";

export default function LoginPage() {

    return (
        <Flex className="lg:h-screen lg:w-screen" gap="0" justifyContent="center" alignItems="center">
            <Grid className="lg:grid-cols-2 border rounded" minHeight="sm:h-[100vh] lg:h-[60vh]" gap="0">
                <Flex direction="column" className="p-6 md:p-10">
                    <Flex justifyContent="center" className="md:justify-start">
                    <a href="#" className="flex items-center gap-2 font-medium">
                        <Flex className="h-6 w-6 rounded-md bg-primary text-primary-foreground" alignItems="center" justifyContent="center">
                            <GalleryVerticalEnd className="size-4" />
                        </Flex>
                    Acme Inc.
                    </a>
                </Flex>
                <Flex className="flex-1" alignItems="center" justifyContent="center">
                    <div className="w-full max-w-xs">
                        <LoginForm />
                    </div>
                </Flex>
                </Flex>
            <div className="relative hidden bg-muted lg:block">
            <img
                src="/placeholder.svg"
                alt="Image"
                className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
            />
            </div>
        </Grid>
      </Flex>
    )
}