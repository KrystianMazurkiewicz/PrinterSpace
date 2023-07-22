import { token } from "data/constants";
import { baseURL } from "data/constants";

export default async function editQAndA(id, question, answer, abortController) {
  const options = {
    signal: abortController?.signal,
    method: "PATCH",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      id: id,
      question: question,
      answers: answer,
    }),
  };

  return fetch(`${baseURL}Admin/EditQAndA`, options)
    .then((response) => {
      if (!response.ok)
        throw new Error(`HTTP error, status = ${response.status}`);
      return response.json();
    })
    .then((data) => data)
    .catch((error) => console.error(error));
}
