import TextField from "@mui/material/TextField";
import { FormRegisterProps } from "./CustomForm";
import { useEffect, useState } from "react";

type TextFieldProps = {
  minLength: number;
  format?: string;
  type?: string;
} & FormRegisterProps;

export const CustomTextField = ({
  id,
  type,
  label,
  format,
  register,
  minLength,
  formValues,
  formValidations,
}: TextFieldProps) => {
  const [touched, setTouched] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    register?.(id, formValues?.[id] ?? "", formValues?.id !== undefined);
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
        type={type}
        error={!formValidations?.[id] && touched}
        helperText={!formValidations?.[id] && touched ? errorMessage : ""}
        required
        fullWidth
        onChange={e => {
          const value = e.target.value;

          const isNumber = type === "number";
          const isUUID = format === "UUID";

          const requiredOk = value.trim() !== "" && value.length >= minLength;
          const uuidOk = !isUUID || UUIDv1.test(value) || UUIDv2.test(value);
          const numberOk = !isNumber || Number.isFinite(Number(value));
          const validated = requiredOk && uuidOk && numberOk;

          const message = !requiredOk
            ? `Required. Must be at least ${minLength} character(s).`
            : !uuidOk
            ? "Invalid UUID"
            : !numberOk
            ? "Must be a number"
            : "";

          setErrorMessage(message);
          register?.(id, value, validated);
          setTouched(true);
        }}
        value={formValues?.[id] ?? ""}
      />
    </>
  );
  return el;
};
