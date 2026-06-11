import Link from "next/link";
import { buttonVariants } from "@/components/ui/button";

export default function NotFound() {
  return (
    <div className="flex flex-col items-center justify-center gap-4 py-24 text-center">
      <p className="font-display text-6xl font-bold text-gold-gradient">404</p>
      <p className="text-muted-foreground">Cette page de l&apos;arène n&apos;existe pas.</p>
      <Link href="/" className={buttonVariants({ variant: "gold" })}>
        Retour à l&apos;arène
      </Link>
    </div>
  );
}
