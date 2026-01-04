import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";
import dayjs, { Dayjs } from "dayjs";
import { FormRegisterProps } from "./CustomForm";
import { useEffect, useState } from "react";

export const CustomTimeAndDateSelector = ({
  label,
  id,
  register,
  formValues,
  formValidations,
}: FormRegisterProps) => {
  const [touched, setTouched] = useState(false);
  const showError = touched && formValidations?.[id] === false;

  useEffect(() => {
    register?.(id, formValues?.[id] ?? "", formValues?.id !== undefined);
  }, []);

  const el = (
    <DateTimePicker
      label={label}
      value={dayjs(formValues?.[id])}
      onChange={(newValue: Dayjs | null) => {
        const validated = !!newValue && newValue.isValid();
        register?.(id, newValue ? newValue.toISOString() : "", validated);
        setTouched(true);
      }}
      closeOnSelect
      format="YYYY-MM-DD hh:mm A" // 12 hour format with AM/PM, can change to 24 hour by using "HH:mm"
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
  );
  return el;
};
