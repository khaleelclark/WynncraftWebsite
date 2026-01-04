import TextField from "@mui/material/TextField";
import { FormRegisterProps } from "./CustomForm";
import { useEffect, useState } from "react";

export const CustomTextField = ({
  id,
  label,
  register,
  formValues,
  formValidations,
}: FormRegisterProps) => {
  const [touched, setTouched] = useState(false);

  useEffect(() => {
    register?.(id, "", false);
  }, []);

  const el = (
    <>
      <TextField
        id={id}
        label={label}
        variant="outlined"
        error={!formValidations?.[id] && touched}
        helperText={
          !formValidations?.[id] && touched ? "This field is required" : ""
        }
        required
        fullWidth
        onChange={e => {
          const value = e.target.value;
          const validated = value !== "" && value.length > 10;
          register?.(id, e.target.value, validated);
          setTouched(true);
        }}
        value={formValues?.[id] ?? ""}
      />
    </>
  );
  return el;
};
