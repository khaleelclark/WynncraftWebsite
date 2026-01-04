import TextField from "@mui/material/TextField";
import { FormRegisterProps } from "./CustomForm";
import { useEffect, useState } from "react";

type TextFieldProps = {
  minLength: number;
  format?: string;
} & FormRegisterProps;

export const CustomTextField = ({
  id,
  label,
  format,
  register,
  minLength,
  formValues,
  formValidations,
}: TextFieldProps) => {
  const [touched, setTouched] = useState(false);

  useEffect(() => {
    register?.(id, "", false);
  }, []);

  const UUIDv1 =
    /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
  const UUIDv2 = /^[0-9a-f]{32}$/i;

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
          const hasValue = value !== "" && value.length >= minLength;

          const uuidValid = UUIDv1.test(value) || UUIDv2.test(value);

          const validated =
            format === "UUID" ? hasValue && uuidValid : hasValue;

          register?.(id, e.target.value, validated);
          setTouched(true);
        }}
        value={formValues?.[id] ?? ""}
      />
    </>
  );
  return el;
};
