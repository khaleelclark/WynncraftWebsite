import axios from "axios";
import { oidc } from "./auth/config";

export const adminApi = axios.create();

adminApi.interceptors.request.use(
  async (config) => {
    const user = await oidc.getUser();
    if (user && !user.expired) {
      config.headers.Authorization = `Bearer ${user.access_token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

adminApi.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      window.dispatchEvent(new CustomEvent("sessionExpired"));
    }
    return Promise.reject(error);
  }
);