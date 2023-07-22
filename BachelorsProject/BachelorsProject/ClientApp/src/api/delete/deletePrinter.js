import { token } from "data/constants";
import { baseURL } from "data/constants";

export default async function deletePrinter(printerName, abortController) {
  const options = {
    signal: abortController?.signal,
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  };

  return fetch(
    `${baseURL}Admin/DeletePrinter?inPrinterName=${printerName}`,
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
