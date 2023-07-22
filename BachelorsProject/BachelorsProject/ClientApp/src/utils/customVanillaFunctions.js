import { v4 as uuidv4 } from "uuid";

export function qs(name) {
  return document.querySelector(name);
}

export function qsa(name) {
  return document.querySelectorAll(name);
}

export function id(name) {
  return document.getElementById(name);
}

export function ls(name, data) {
  return localStorage.setItem(name, js(data));
}

export function lg(name) {
  return localStorage.getItem(name);
}

export function lgp(name) {
  return jp(localStorage.getItem(name));
}

export function js(data) {
  return JSON.stringify(data);
}

export function jp(data) {
  return JSON.parse(data);
}

export function log(message) {
  console.log(message);
}

export function uniqueId() {
  return uuidv4();
}
