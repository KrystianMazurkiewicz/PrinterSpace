export default function calculateTimeDifference(dateString1, dateString2) {
  const date1 = new Date(dateString1);
  const date2 = new Date(dateString2);

  const timeDiff = date1.getTime() - date2.getTime();

  let hoursDiff = Math.floor(timeDiff / (1000 * 60 * 60));
  let minutesDiff = Math.floor((timeDiff % (1000 * 60 * 60)) / (1000 * 60));

  if (minutesDiff < 0) {
    minutesDiff += 60;
    hoursDiff -= 1;
  }

  if (hoursDiff < 0) {
    hoursDiff += 24;
  }

  return `${hoursDiff} hours ${minutesDiff} minutes`;
}
