import { UserManager } from "oidc-client-ts";

export const oidc = new UserManager({
  authority: "http://localhost:9000/application/o/imperial-web/",
  client_id: "HQkmNiwI0QSHYRPAgDvLCUYxcEnPl2dn2x9KTUV8",
  redirect_uri: "http://localhost:5173/",
  response_type: "code",
  scope: "openid profile email",
});