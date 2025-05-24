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

export const unixToDateString = (unixTimestamp: number): string => {
  const date = new Date(unixTimestamp * 1000);
  
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const year = date.getFullYear();
  
  return `${hours}:${minutes} ${day}-${month}-${year}`;
};

export const formatDuration = (seconds: number): string => {
  if (seconds < 60) {
    return `${seconds}s`;
  }
  
  const days = Math.floor(seconds / 86400);
  const hours = Math.floor((seconds % 86400) / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const remainingSeconds = seconds % 60;
  
  if (days > 0) {
    if (hours > 0) {
      return `${days}d ${hours}h ${minutes}m`;
    } else if (minutes > 0) {
      return `${days}d ${minutes}m`;
    } else {
      return `${days}d`;
    }
  } else if (hours > 0) {
    if (minutes > 0) {
      return `${hours}h ${minutes}m`;
    } else {
      return `${hours}h`;
    }
  } else {
    return `${minutes}m ${remainingSeconds}s`;
  }
};
