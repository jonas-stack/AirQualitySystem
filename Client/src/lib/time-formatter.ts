export const formatLastUpdated = (date: Date) => {
    return date.toLocaleString("en-US", {
        hour: "numeric",
        minute: "numeric",
        hour12: true,
        month: "short",
        day: "numeric",
    })
}

export function convertDotNetTicksToDate(ticks: number): Date {
  const jsTicks = ticks - 621355968000000000;
  const msSinceEpoch = jsTicks / 10000;
  return new Date(msSinceEpoch);
}

export function convertDotNetTicksToDateString(ticks: number): string {
  return convertDotNetTicksToDate(ticks).toISOString();
}

export function formatTicksToUTC(ticks: number): string {
  const jsTicks = ticks - 621355968000000000;
  const msSinceEpoch = jsTicks / 10000;
  const date = new Date(msSinceEpoch);

  const pad = (n: number) => n.toString().padStart(2, "0");

  const hours = pad(date.getUTCHours());
  const minutes = pad(date.getUTCMinutes());
  const day = pad(date.getUTCDate());
  const month = pad(date.getUTCMonth() + 1);
  const year = date.getUTCFullYear();

  return `${hours}:${minutes} ${day}-${month}-${year}`;
}