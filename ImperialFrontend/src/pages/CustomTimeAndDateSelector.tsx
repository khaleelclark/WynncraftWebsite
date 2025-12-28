import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";
import { Dayjs } from "dayjs";
import { useCustomFormContext } from "../CustomFormContext";

interface DateTimeSelectorProps {
  value: Dayjs | null;
  onChange: (value: Dayjs | null) => void;
  label: string;
  id: string;
}

export const CustomTimeAndDateSelector = ({
  label,
  value,
  id,
  onChange,
}: DateTimeSelectorProps) => {
  const { register } = useCustomFormContext();

  const handleChange = (newValue: Dayjs | null) => {
    // update local state in parent (e.g. FormTest)
    onChange(newValue);

    // register ISO string (UTC) for the payload -> good for DateTimeOffset
    register(id, newValue ? newValue.toISOString() : "");
  };

  return (
    <DateTimePicker
      label={label}
      value={value}
      onChange={handleChange}
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
