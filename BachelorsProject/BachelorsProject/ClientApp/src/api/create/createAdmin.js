import { token } from "data/constants";
import { baseURL } from "data/constants";

export default function createAdmin(email, abortController) {
  const options = {
    signal: abortController?.signal,
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      userName: email.split("@")[0],
      email: email,
      role: 0,
    }),
  };

  return fetch(`${baseURL}Admin/CreateAdmin`, options)
    .then((response) => {
      if (!response.ok)
        throw new Error(`HTTP error, status = ${response.status}`);
      return response.json();
    })
    .then((data) => data)
    .catch((error) => console.error(error));
}
