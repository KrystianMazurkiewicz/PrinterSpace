import { nameByToken, token } from "data/constants";
import { baseURL } from "data/constants";

export default async function uploadFile(file, username, abortController) {
  const formData = new FormData();
  formData.append("inFile", file);

  const options = {
    signal: abortController?.signal,
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: formData,
  };

  return fetch(`${baseURL}User/Upload?userName=${nameByToken}`, options)
    .then((response) => {
      if (!response.ok)
        throw new Error(`HTTP error, status = ${response.status}`);
      return response.json();
    })
    .then((data) => data)
    .catch((error) => console.error(error));
}
