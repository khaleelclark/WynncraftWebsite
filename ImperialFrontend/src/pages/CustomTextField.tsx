import TextField from "@mui/material/TextField";
import { useCustomFormContext } from "../CustomFormContext";

interface TextFieldProps {
  id: string;
  label: string;
  required?: boolean;
}

export const CustomTextField = ({ id, label, required }: TextFieldProps) => {
  const { register, formValues } = useCustomFormContext();
  const el = (
    <>
      <TextField
        id={id}
        label={label}
        variant="outlined"
        required={required}
        fullWidth
        onChange={e => register(id, e.target.value)}
        value={formValues[id] ?? ""}
      />
    </>
  );
  return el;
};
