import React, { useEffect } from "react";
import jwt from "jwt-decode";
import { useLocation } from "react-router-dom";
import Cookies from "universal-cookie";
import login from "api/read/login";

export default function Register() {
  const location = useLocation();

  useEffect(() => {
    const abortController = new AbortController();

    const fetchData = async () => {
      const searchParams = new URLSearchParams(location.search);
      const queryParams = {};

      for (const [key, value] of searchParams.entries()) {
        queryParams[key] = value;
      }

      const token = await login(
        queryParams.username,
        queryParams.password,
        abortController
      );

      const cookies = new Cookies();
      const decoded = jwt(token);

      cookies.set("printerspace_jwt", token, {
        expires: new Date(decoded.exp * 1000),
      });

      window.location.href = "/";
    };

    fetchData();

    return () => {
      abortController.abort();
    };
  }, [location.search]);

  return null;
}
