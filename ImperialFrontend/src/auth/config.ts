import { UserManager } from "oidc-client-ts";

export const oidc = new UserManager({
  authority: "http://localhost:9000/application/o/imperial-web/",
  client_id: "Aaikh5UvU2JHg0EuBCQMfDNJNxO6cbko0FxxCg03",
  redirect_uri: "http://localhost:5173/",
  response_type: "code",
  scope: "openid profile email",
});
