import { token } from "data/constants";
import { baseURL } from "data/constants";

export default async function togglePrinterLock(
  printerName,
  lockStatus,
  abortController
) {
  const newLockStatus = lockStatus === "Locked" ? "unlock" : "lock";
  const options = {
    signal: abortController?.signal,
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  };

  return fetch(
    `${baseURL}Admin/UnlockAndLockPrinter?inCommand=${newLockStatus}&printerName=${printerName}`,
    options
  )
    .then((response) => {
      if (!response.ok)
        throw new Error(`HTTP error, status = ${response.status}`);
      return response.json();
    })
    .then((data) => data)
    .catch((error) => console.error(error));
}
