export default function convertSeconds(seconds) {
  let minutes = Math.floor(seconds / 60);
  let hours = Math.floor(minutes / 60);
  let remainingMinutes = minutes % 60;

  if (minutes < 59) return minutes + " minutes";
  return hours + " hours " + remainingMinutes + " minutes";
}
