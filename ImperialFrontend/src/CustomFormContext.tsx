import { createContext, useContext } from "react";

type CustomFormContextType = {
  register: (id: string, value: string) => void;
  formValues: Record<string, any>;
};

export const CustomFormContext = createContext<CustomFormContextType | null>(
  null
);

export const useCustomFormContext = () => {
  const ctx = useContext(CustomFormContext);
  if (!ctx) {
    throw new Error(
      "Form control components must be used within a Custom Form component."
    );
  }
  return ctx;
};
