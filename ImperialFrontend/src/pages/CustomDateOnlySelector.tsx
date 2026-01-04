import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import dayjs, { Dayjs } from "dayjs";
import { FormRegisterProps } from "./CustomForm";
import { useEffect, useState } from "react";

export const CustomDateOnlySelector = ({
  label,
  id,
  register,
  formValues,
  formValidations,
}: FormRegisterProps) => {
  const [touched, setTouched] = useState(false);
  const showError = touched && formValidations?.[id] === false;

  useEffect(() => {
    register?.(id, "", false);
  }, []);

  const el = (
    <>
      <DatePicker
        label={label}
        value={dayjs(formValues?.[id])}
        onChange={(newValue: Dayjs | null) => {
          const validated = !!newValue && newValue.isValid();
          register?.(
            id,
            newValue ? newValue.format("YYYY-MM-DD") : "",
            validated
          );
          setTouched(true);
        }}
        closeOnSelect
        format="YYYY-MM-DD"
        slotProps={{
          textField: {
            name: id,
            fullWidth: true,
            required: true,
            error: showError,
            helperText: showError ? "This field is required" : "",
            onBlur: () => setTouched(true),
          },
        }}
      />
    </>
  );
  return el;
};
