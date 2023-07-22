import Cookies from "universal-cookie";
import jwt from "jwt-decode";

const roleObjectKey =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
const nameObjectKey =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

const cookies = new Cookies();
const jwtCookie = cookies.get("printerspace_jwt");
const decodedJwt = jwtCookie ? jwt(jwtCookie) : {};

export const token = jwtCookie;
export const roleByToken = decodedJwt[roleObjectKey];
export const nameByToken = decodedJwt[nameObjectKey];
export const baseURL = "http://localhost:5265/";

export const ROLES = {
  admin: "Admin",
  user: "User",
};
