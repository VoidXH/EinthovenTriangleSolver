using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EinthovenTriangleSolver {
    public partial class MainWindow : Window {
        private readonly Brush foreground, used;
        object iSource, iiSource, iiiSource, avrSource, avfSource, avlSource; // Calculated field pairs
        private readonly double[] betas = { double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN }, // Cached angle constants
            lasts = { double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN }; // Cached results for solutions

        public MainWindow() {
            InitializeComponent();
            foreground = helpAlpha.Foreground;
            used = new SolidColorBrush(Color.FromRgb(148 / 2, 192 / 2, 255));
        }

        bool editing = false;
        readonly HashSet<object> edited = new HashSet<object>();
        object firstSender;

        void StartEditing(object sender) {
            if (!editing) {
                editing = true;
                firstSender = sender;
            }
            if (!edited.Contains(sender))
                edited.Add(sender);
        }

        void EndEditing(object sender) {
            if (firstSender == sender) {
                editing = false;
                edited.Clear();
            }
        }

        bool Parse(TextBox from, out double value) {
            if (from != null)
                return double.TryParse(from.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            else {
                value = double.NaN;
                return false;
            }
        }

        bool ParseFix(TextBox from, out double value) {
            if (Parse(from, out value)) {
                if (!edited.Contains(from))
                    edited.Add(from);
                return true;
            }
            return false;
        }

        string Display(double value) => value.ToString("0.000000").TrimEnd('0').TrimEnd(',', '.');

        void Apply(TextBox to, double value) {
            if (to != null && !edited.Contains(to)) {
                to.Text = Display(value);
                edited.Add(to);
            }
        }

        void ResetMarkings() => helpAlpha.Foreground = helpTg.Foreground = helpSinAmB.Foreground = helpCosAmB.Foreground =
            sinSubForCosA.Foreground = sinSubForCosB.Foreground = sinSubForSinA.Foreground =
            sinSubForSinB.Foreground = cosSubForCos.Foreground = cosSubForSin.Foreground = foreground;

        Brush ResetMarkings(object sender, Label markedLabel = null, Button markedHelp = null) {
            if (Parse(tga, out _)) {
                if (firstSender == sender) {
                    ResetMarkings();
                    if (markedLabel != null)
                        markedLabel.Foreground = used;
                    if (markedHelp != null)
                        markedHelp.Foreground = used;
                    return used;
                }
            }
            return foreground;
        }

        double Sin(double value) => Math.Sin(value * Math.PI / 180);
        double Cos(double value) => Math.Cos(value * Math.PI / 180);

        void SolutionDisplay(string solution, char substitute, char otherVar, double lastSubstitute, double lastX) {
            if (double.IsNaN(lastSubstitute) || double.IsNaN(lastX))
                MessageBox.Show(solution, "Solution");
            else
                MessageBox.Show(solution.Replace($"{substitute}", Display(lastSubstitute) + "°")
                    .Replace(otherVar, 'α').Replace("x", Display(lastX)), "Solution");
        }

        double SinSubPerSinA(double beta, double x) => Sin(betas[0] = beta) / (Cos(beta) - (lasts[0] = x)); // tg(α) for sin(α - β) / sin(α) = x
        double SinSubPerSinB(double alpha, double x) => Sin(betas[1] = alpha) / ((lasts[1] = x) + Cos(alpha)); // tg(β) for sin(α - β) / sin(β) = x
        double SinSubPerCosA(double beta, double x) => ((lasts[2] = x) + Sin(betas[2] = beta)) / Cos(beta); // tg(α) for sin(α - β) / cos(α) = x
        double SinSubPerCosB(double alpha, double x) => (Sin(betas[3] = alpha) - (lasts[3] = x)) / Cos(alpha); // tg(β) for sin(α - β) / cos(β) = x
        double CosSubPerCos(double any, double x) => ((lasts[4] = x) - Cos(betas[4] = any)) / Sin(any); //  tg(α) for cos(α - β) / cos(α or β) = x
        double CosSubPerSin(double any, double x) => Cos(betas[5] = any) / ((lasts[5] = x) - Sin(any)); // tg(α) for cos(α - β) / sin(α or β) = x

        void SinSubPerSinA(object sender, RoutedEventArgs _) => SolutionDisplay(Solutions.formSinSubSinA, 'β', 'α', betas[0], lasts[0]);
        void SinSubPerSinB(object sender, RoutedEventArgs _) => SolutionDisplay(Solutions.formSinSubSinB, 'α', 'β', betas[1], lasts[1]);
        void SinSubPerCosA(object sender, RoutedEventArgs _) => SolutionDisplay(Solutions.formSinSubCosA, 'β', 'α', betas[2], lasts[2]);
        void SinSubPerCosB(object sender, RoutedEventArgs _) => SolutionDisplay(Solutions.formSinSubCosB, 'α', 'β', betas[3], lasts[3]);
        void CosSubPerCos(object sender, RoutedEventArgs _) => SolutionDisplay(Solutions.formCosSubCos, 'β', 'α', betas[4], lasts[4]);
        void CosSubPerSin(object sender, RoutedEventArgs _) => SolutionDisplay(Solutions.formCosSubSin, 'β', 'α', betas[5], lasts[5]);

        bool HandleSource(ref object source, object sender, TextBox parsed, TextChangedEventHandler changer, Action<object> solver) {
            if ((source == null || source == parsed) && ParseFix(parsed, out _)) {
                source = parsed;
                solver(sender);
                return true;
            } else if (source == parsed) {
                source = null;
                changer(sender, null);
            }
            return false;
        }

        void ApplyOutputs(double _a, double _e) {
            Apply(i, _e * Cos(_a));
            Apply(ii, _e * Cos(60 - _a));
            Apply(iii, _e * Sin(_a - 30));
            Apply(avr, _e * Cos(_a - 30));
            Apply(avl, _e * Sin(60 - _a));
            Apply(avf, _e * Sin(_a));
        }

        void SolveFromAlphaE(double _tga, double _e, TextBox num, TextBox den) {
            double _a = Math.Atan(_tga) * 180 / Math.PI;
            Apply(tga, _tga);
            Apply(a, _a);
            if (double.IsNaN(_e) || (num != firstSender && den != firstSender))
                return;
            ApplyOutputs(_a, _e);
            e.Text = Display(_e);
        }

        void SolvePair(TextBox num, TextBox den, Func<double, double> tangent, object sender, Label formula, Button help) {
            Parse(num, out double _num);
            Parse(den, out double _denom);
            double _tga = tangent(_num / _denom);
            if (double.IsNaN(_tga) || double.IsInfinity(_tga))
                return;
            if (den == i)
                SolveFromAlphaE(_tga, _denom / Math.Cos(Math.Atan(_tga)), den, num);
            else
                SolveFromAlphaE(_tga, _denom / Math.Sin(Math.Atan(_tga)), num, den);
            helpAlpha.Foreground = helpTg.Foreground = ResetMarkings(sender, formula, help);
            tangent(_num / _denom); // Makes sure the marked help will have a correct x value
        }

        void SolveFromInII(object sender) => SolvePair(ii, i, x => CosSubPerCos(60, x), sender, helpCosAmB, cosSubForCos);
        void SolveFromInIII(object sender) => SolvePair(iii, i, x => SinSubPerCosA(30, x), sender, helpSinAmB, sinSubForCosA);
        void SolveFromInAVR(object sender) => SolvePair(avr, i, x => CosSubPerCos(30, x), sender, helpCosAmB, cosSubForCos);
        void SolveFromInAVL(object sender) => SolvePair(avl, i, x => SinSubPerCosB(60, x), sender, helpSinAmB, sinSubForCosB);
        void SolveFromInAVF(object sender) => SolvePair(avf, i, x => x, sender, helpTg, null);
        void SolveFromIInAVF(object sender) => SolvePair(ii, avf, x => CosSubPerSin(60, x), sender, helpCosAmB, cosSubForSin);
        void SolveFromIIInAVF(object sender) => SolvePair(iii, avf, x => SinSubPerSinA(30, x), sender, helpSinAmB, sinSubForSinA);
        void SolveFromAVRnAVF(object sender) => SolvePair(avr, avf, x => CosSubPerSin(30, x), sender, helpCosAmB, cosSubForSin);
        void SolveFromAVLnAVF(object sender) => SolvePair(avl, avf, x => SinSubPerSinB(60, x), sender, helpSinAmB, sinSubForSinB);

        void SolveI(object sender, TextChangedEventArgs _e) {
            StartEditing(sender);
            if (Parse(i, out _))
                _ = HandleSource(ref iSource, sender, ii, SolveI, SolveFromInII) ||
                    HandleSource(ref iSource, sender, iii, SolveI, SolveFromInIII) ||
                    HandleSource(ref iSource, sender, avr, SolveI, SolveFromInAVR) ||
                    HandleSource(ref iSource, sender, avl, SolveI, SolveFromInAVL) ||
                    HandleSource(ref iSource, sender, avf, SolveI, SolveFromInAVF);
            EndEditing(sender);
        }

        void MiddleField(object sender, ref object source, TextChangedEventHandler handler, Action<object> withI, Action<object> withAVF) {
            StartEditing(sender);
            if (Parse((TextBox)sender, out _))
                _ = HandleSource(ref source, sender, i, handler, withI) || HandleSource(ref source, sender, avf, handler, withAVF);
            EndEditing(sender);
        }

        void SolveII(object sender, TextChangedEventArgs _) => MiddleField(sender, ref iiSource, SolveII, SolveFromInII, SolveFromIInAVF);
        void SolveIII(object sender, TextChangedEventArgs _) => MiddleField(sender, ref iiiSource, SolveIII, SolveFromInIII, SolveFromIIInAVF);
        void SolveAVR(object sender, TextChangedEventArgs _) => MiddleField(sender, ref avrSource, SolveAVR, SolveFromInAVR, SolveFromAVRnAVF);
        void SolveAVL(object sender, TextChangedEventArgs _) => MiddleField(sender, ref avlSource, SolveAVL, SolveFromInAVL, SolveFromAVLnAVF);

        void SolveAVF(object sender, TextChangedEventArgs _e) {
            StartEditing(sender);
            if (Parse(avf, out _))
                _ = HandleSource(ref avfSource, sender, i, SolveAVF, SolveFromInAVF) ||
                    HandleSource(ref avfSource, sender, ii, SolveAVF, SolveFromIInAVF) ||
                    HandleSource(ref avfSource, sender, iii, SolveAVF, SolveFromIIInAVF) ||
                    HandleSource(ref avfSource, sender, avr, SolveAVF, SolveFromAVRnAVF) ||
                    HandleSource(ref avfSource, sender, avl, SolveAVF, SolveFromAVLnAVF);
            EndEditing(sender);
        }

        void SolveAlphaE() {
            if (!ParseFix(a, out double _a) || !ParseFix(e, out double _e))
                return;
            ParseFix(tga, out _);
            if (firstSender == a || firstSender == tga || firstSender == e) {
                ApplyOutputs(_a, _e);
                ResetMarkings();
                helpAlpha.Foreground = used;
            }
        }

        void SolveAlpha(object sender, TextChangedEventArgs _) {
            StartEditing(sender);
            if (Parse(a, out double _a)) {
                eVector.RenderTransform = new RotateTransform(_a);
                iMarker.Foreground = iArr1.Background = iArr2.Background = _a < 30 ? used : foreground;
                iiMarker.Foreground = iiArr1.Background = iiArr2.Background = _a > 30 ? used : foreground;
                avrMarker.Foreground = avrArr.Background = avrArr1.Background = avrArr2.Background =
                eMarker.Foreground = eArr.Background = eArr1.Background = eArr2.Background =
                    iiiMarker.Foreground = iiiArr1.Background = iiiArr2.Background = alphaMarker.Foreground = used;
                Apply(tga, Math.Tan(_a * Math.PI / 180));
                SolveAlphaE();
            }
            EndEditing(sender);
        }

        void SolveTgAlpha(object sender, TextChangedEventArgs _) {
            StartEditing(sender);
            if (Parse(tga, out double _tga))
                Apply(a, Math.Atan(_tga) * 180 / Math.PI);
            EndEditing(sender);
        }

        void SolveE(object sender, TextChangedEventArgs _) {
            StartEditing(sender);
            SolveAlphaE();
            EndEditing(sender);
        }

        void Reset(object sender, RoutedEventArgs _) {
            iSource = iiSource = iiiSource = avrSource = avlSource = avfSource = null;
            i.Text = ii.Text = iii.Text = avr.Text = avl.Text = avf.Text = tga.Text = a.Text = e.Text = string.Empty;
            ResetMarkings();
        }
    }
}