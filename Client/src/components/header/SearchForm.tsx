
import { Button } from "../ui/button"
interface SearchFormProps {
  setOpen: React.Dispatch<React.SetStateAction<boolean>>
}

export function SearchForm({ setOpen }: SearchFormProps) {
    return (
      <div className="hidden w-full flex-1 md:flex md:w-auto md:flex-none">
        <Button
          size="sm"
          className="cursor-pointer inline-flex items-center gap-2 whitespace-nowrap transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0 border border-input hover:bg-accent hover:text-accent-foreground px-4 py-2 relative h-8 w-full justify-start rounded-[0.5rem] bg-muted/50 text-sm font-normal text-muted-foreground shadow-none sm:pr-12 md:w-40 lg:w-56 xl:w-64"
          onClick={() => setOpen(true)}
        >
          <span className="hidden lg:inline-flex">Search website...</span>
          <span className="inline-flex lg:hidden">Search...</span>
          <kbd className="pointer-events-none absolute right-[0.3rem] top-[0.3rem] hidden h-5 select-none items-center gap-1 rounded border bg-muted px-1.5 font-mono text-[10px] font-medium opacity-100 sm:flex">
            <span className="text-xs">⌘</span>S
          </kbd>
        </Button>
      </div>
    );
  }