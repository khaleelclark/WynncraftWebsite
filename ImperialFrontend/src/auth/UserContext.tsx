import React, { createContext, useContext, useEffect, useState } from "react";
import { User } from "oidc-client-ts";
import { oidc } from "./config";

interface UserContextType {
  user: User | null;
  isLoading: boolean;
  signinRedirect: () => void;
  signoutRedirect: () => void;
  signoutLocal: () => Promise<void>;
}

const UserContext = createContext<UserContextType>({
  user: null,
  isLoading: true,
  signinRedirect: () => {},
  signoutRedirect: () => {},
  signoutLocal: () => Promise.resolve(),
});

export const UserProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const processInitialAuth = async () => {
      const params = new URLSearchParams(window.location.search);
      if (params.has("code") && params.has("state")) {
        try {
          await oidc.signinRedirectCallback();
          window.history.replaceState({}, document.title, window.location.pathname);
        } catch (e) {
          console.error("Error handling OIDC callback", e);
          setIsLoading(false);
        }
      } else {
        try {
          const existingUser = await oidc.getUser();
          if (existingUser) {
            setUser(existingUser);
          }
        } catch (e) {
            console.error("Error getting user", e)
        }
        setIsLoading(false);
      }
    };

    processInitialAuth();

    const onUserLoaded = (loadedUser: User) => {
      setUser(loadedUser);
      setIsLoading(false);
    };

    const onUserUnloaded = () => {
      setUser(null);
      setIsLoading(false);
    };

    oidc.events.addUserLoaded(onUserLoaded);
    oidc.events.addUserUnloaded(onUserUnloaded);
    oidc.events.addSilentRenewError(() => setIsLoading(false));
    oidc.events.addUserSignedOut(() => setIsLoading(true));


    return () => {
      oidc.events.removeUserLoaded(onUserLoaded);
      oidc.events.removeUserUnloaded(onUserUnloaded);
      oidc.events.removeSilentRenewError(() => setIsLoading(false));
      oidc.events.removeUserSignedOut(() => setIsLoading(true));
    };
  }, []);

  const signinRedirect = async () => {
    setIsLoading(true);
    await oidc.signinRedirect();
  };

  const signoutRedirect = async () => {
    setIsLoading(true);
    await oidc.signoutRedirect({
      post_logout_redirect_uri: "http://localhost:5173/",
    });
  };

  const signoutLocal = async () => {
    setIsLoading(true);
    await oidc.removeUser();
  };

  return (
    <UserContext.Provider
      value={{
        user,
        isLoading,
        signinRedirect,
        signoutRedirect,
        signoutLocal,
      }}
    >
      {children}
    </UserContext.Provider>
  );
};

export const useUser = () => useContext(UserContext);
