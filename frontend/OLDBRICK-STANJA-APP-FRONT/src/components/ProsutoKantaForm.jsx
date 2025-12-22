import { useState } from "react";
import { putMeasuredProsuto, calculateProsutoRazlika } from "../api/helpers";

function ProsutoKantaForm({ idNaloga, onSaved }) {
  const [value, setValue] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log("SUBMIT", idNaloga, value);
    if (!idNaloga) return;

    const num = Number(value);
    if (Number.isNaN(num)) return;

    await putMeasuredProsuto(idNaloga, num);
    await calculateProsutoRazlika(idNaloga);
    setValue("");
    onSaved?.();
  };
  return (
    <form onSubmit={handleSubmit} className="mt-4 space-y-2">
      <label className="block text-sm text-white/70">Prosuto (kanta)</label>

      <div className="flex gap-2">
        <input
          type="number"
          step="0.01"
          value={value}
          onChange={(e) => setValue(e.target.value)}
          className="w-full rounded-lg bg-white/5 border border-white/10 px-3 py-2 text-white outline-none focus:ring-2 focus:ring-yellow-400/40"
          placeholder="npr. 0.70"
        />

        <button
          type="submit"
          className="rounded-lg px-4 py-2 bg-[#FACC15] text-black font-semibold hover:opacity-90 transition"
        >
          Sačuvaj
        </button>
      </div>
    </form>
  );
}

export default ProsutoKantaForm;
