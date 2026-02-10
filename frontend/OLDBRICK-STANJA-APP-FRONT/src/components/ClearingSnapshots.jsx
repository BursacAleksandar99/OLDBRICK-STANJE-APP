import { useMemo, useState } from "react";
import { upsertCleaningSnapshotsBatch } from "../api/helpers";

function CleaningSnapshotsForm({
  idNaloga,
  datum,
  beers,
  onSaved,
  isReportCalculated,
}) {
  // beers: [{ idPiva, naziv }]

  const [values, setValues] = useState({}); // { [idPiva]: number }
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const canSubmit = useMemo(() => {
    return (
      idNaloga &&
      datum &&
      !isReportCalculated &&
      Object.keys(values).length > 0 &&
      Object.values(values).every(
        (v) => typeof v === "number" && !Number.isNaN(v) && v >= 0,
      )
    );
  }, [idNaloga, datum, values, isReportCalculated]);

  function handleChange(idPiva, raw) {
    const n = raw === "" ? "" : Number(raw);

    setValues((prev) => ({
      ...prev,
      [idPiva]: n,
    }));
  }

  async function handleSubmit() {
    setError("");

    try {
      setLoading(true);

      const items = Object.entries(values)
        .filter(([, v]) => v !== "" && !Number.isNaN(v))
        .map(([idPiva, brojacStart]) => ({
          idPiva: Number(idPiva),
          brojacStart: Number(brojacStart),
        }));

      await upsertCleaningSnapshotsBatch({
        datum,
        idNaloga,
        items,
      });

      onSaved?.();
    } catch (e) {
      setError(e?.message || "Greška pri čuvanju cleaning-a.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="space-y-3">
      <div className="text-sm font-semibold text-white">Pranje točilica</div>

      <div className="space-y-2">
        {beers.map((b) => (
          <div
            key={b.idPiva}
            className="flex items-center justify-between gap-3"
          >
            <div className="text-slate-200">{b.naziv}</div>

            <input
              type="number"
              min="0"
              step="0.01"
              className="w-28 rounded-md bg-slate-900 px-3 py-2 text-slate-100 outline-none ring-1 ring-slate-700"
              value={values[b.idPiva] ?? ""}
              onWheel={(e) => e.currentTarget.blur()}
              onChange={(e) => handleChange(b.idPiva, e.target.value)}
              placeholder="Brojač"
            />
          </div>
        ))}
      </div>

      {error ? <div className="text-sm text-red-400">{error}</div> : null}

      <button
        type="button"
        onClick={handleSubmit}
        disabled={!canSubmit || loading || isReportCalculated}
        className="rounded-lg bg-yellow-400 px-4 py-2 font-semibold text-black disabled:opacity-50"
      >
        {loading ? "Čuvam..." : "Sačuvaj pranje"}
      </button>

      {isReportCalculated && (
        <div className="mt-2 text-xs text-red-600">
          Izmene su dozvoljene samo za nalog koji nije sačuvan!
        </div>
      )}
    </div>
  );
}
export default CleaningSnapshotsForm;
