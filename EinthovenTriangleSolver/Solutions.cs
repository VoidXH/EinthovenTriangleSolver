namespace EinthovenTriangleSolver {
    static class Solutions {
        public const string formSinSubSinA = @"sin(α - β) / sin(α) = x
(sin(α) * cos(β) - cos(α) * sin(β)) / sin(α) = x
sin(α) * cos(β) - cos(β) * sin(β) = x * sin(α)
cos(α) * sin(β) = (cos(β) - x) * sin(α)
cos(α) / sin(α) = (cos(β) - x) / sin(β)
sin(α) / cos(α) = sin(β) / (cos(β) - x)
tg(α) = sin(β) / (cos(β) - x)";

        public const string formSinSubSinB = @"sin(α - β) / sin(β) = x
(sin(α) * cos(β) - cos(α) * sin(β)) / sin(β) = x
sin(α) * cos(β) = (x + cos(α)) * sin(β)
cos(β) / sin(β) = (x + cos(α)) / sin(α)
sin(β) / cos(β) = sin(α) / (x + cos(α))
tg(β) = sin(α) / (x + cos(α))";

        public const string formSinSubCosA = @"sin(α - β) / cos(α) = x
(sin(α) * cos(β) - cos(α) * sin(β)) / cos(α) = x
sin(α) * cos(β) - cos(α) * sin(β) = x * cos(α)
sin(α) * cos(β) = (x + sin(β)) * cos(α)
sin(α) / cos(α) = (x + sin(β)) / cos(β)
tg(α) = (x + sin(β)) / cos(β)";

        public const string formSinSubCosB = @"sin(α - β) / cos(β) = x
(sin(α) * cos(β) - cos(α) * sin(β)) / cos(β) = x
sin(α) * cos(β) - cos(α) * sin(β) = x * cos(β)
cos(α) * sin(β) = (sin(α) - x) * cos(β)
sin(β) / cos(β) = (sin(α) - x) / cos(α)
tg(β) = (sin(α) - x) / cos(α)";

        public const string formCosSubCos = @"cos(α) and cos(β) are interchangable:
cos(α - β) / cos(α) = x
(cos(α) * cos(β) + sin(α) * sin(β)) / cos(α) = x
cos(α) * cos(β) + sin(α) * sin(β) = x * cos(α)
sin(α) * sin(β) = (x - cos(β)) * cos(α)
sin(α) / cos(α) = (x - cos(β)) / sin(β)
tg(α) = (x - cos(β)) / sin(β)";

        public const string formCosSubSin = @"sin(α) and sin(β) are interchangable:
cos(α - β) / sin(α) = x
(cos(α) * cos(β) + sin(α) * sin(β)) / sin(α) = x
cos(α) * cos(β) + sin(α) * sin(β) = x * sin(α)
cos(α) * cos(β) = (x - sin(β)) * sin(α)
cos(α) / sin(α) = (x - sin(β)) / cos(β)
sin(α) / cos(α) = cos(β) / (x - sin(β))
tg(α) = cos(β) / (x - sin(β))";
    }
}