import { token } from "data/constants";
import { baseURL } from "data/constants";

export default async function editPrinter(
  printerName,
  ipAddress,
  apiKey,
  ApiSecret,
  abortController
) {
  const options = {
    signal: abortController?.signal,
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      printerName: printerName,
      ip: ipAddress,
      apiKey: apiKey,
      apiSecret: ApiSecret,
    }),
  };

  return fetch(`${baseURL}Admin/UpdatePrinter`, options)
    .then((response) => {
      if (!response.ok)
        throw new Error(`HTTP error, status = ${response.status}`);
      return response.json();
    })
    .then((data) => data)
    .catch((error) => console.error(error));
}
