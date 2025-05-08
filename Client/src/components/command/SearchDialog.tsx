import { CommandDialog, CommandInput, CommandList, CommandItem, CommandGroup, CommandEmpty, CommandSeparator } from "@/components/ui/command"
import { Calendar, Smile, Calculator, User, CreditCard, Settings } from "lucide-react"
import { Dispatch, SetStateAction } from "react"

interface CommandDialogProps {
  open: boolean
  setOpen: Dispatch<SetStateAction<boolean>>
}

export function SearchDialog({ open, setOpen }: CommandDialogProps) {
  return (
      <CommandDialog open={open} onOpenChange={setOpen}>
        <CommandInput placeholder="Type a command or search..." />
        <CommandList>
          <CommandEmpty>No results found.</CommandEmpty>
          <CommandGroup heading="Suggestions">
            <CommandItem>
              <Calendar />
              <span>Calendar</span>
            </CommandItem>
            <CommandItem>
              <Smile />
              <span>Search Emoji</span>
            </CommandItem>
            <CommandItem>
              <Calculator />
              <span>Calculator</span>
            </CommandItem>
          </CommandGroup>
          <CommandSeparator />
          <CommandGroup heading="Settings">
            <CommandItem>
              <User />
              <span>Profile</span>
            </CommandItem>
            <CommandItem>
              <CreditCard />
              <span>Billing</span>
            </CommandItem>
            <CommandItem>
              <Settings />
              <span>Settings</span>
            </CommandItem>
          </CommandGroup>
        </CommandList>
      </CommandDialog>
  )
}