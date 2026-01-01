import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";
import dayjs, { Dayjs } from "dayjs";
import { useCustomFormContext } from "../CustomFormContext";

interface DateTimeSelectorProps {
  label: string;
  id: string;
}

export const CustomTimeAndDateSelector = ({
  label,
  id,
}: DateTimeSelectorProps) => {
  const { register, formValues } = useCustomFormContext();

  return (
    <DateTimePicker
      label={label}
      value={dayjs(formValues[id])}
      onChange={(newValue: Dayjs | null) =>
        register(id, newValue ? newValue.toISOString() : "")
      }
      format="YYYY-MM-DD hh:mm A" // 12 hour format with AM/PM, can change to 24 hour by using "HH:mm"
      slotProps={{
        textField: {
          name: id,
          fullWidth: true,
        },
      }}
    />
  );
};
