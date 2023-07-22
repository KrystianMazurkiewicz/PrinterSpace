import { token } from "data/constants";
import { baseURL } from "data/constants";

export default async function createQAndA(question, answer, abortController) {
  const options = {
    signal: abortController?.signal,
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      question: question,
      answers: answer,
    }),
  };

  return fetch(`${baseURL}Admin/MakeNewQAndA`, options)
    .then((response) => {
      if (!response.ok)
        throw new Error(`HTTP error, status = ${response.status}`);
      return response.json();
    })
    .then((data) => data)
    .catch((error) => console.error(error));
}
