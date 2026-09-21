namespace ConversorMoedas.Validators;
using FluentValidation;
using ConversorMoedas.Models;

public class ConversaoValidator : AbstractValidator<ConversaoRequest>
{
    public ConversaoValidator()
    {
        RuleFor(x => x.De).NotEmpty().WithMessage("Moeda de origem é obrigatória!")
            .Length(3).WithMessage("Código da moeda deve ter 3 caracteres!")
            .Matches("^[A-Z]{3}$").WithMessage("Código deve ser em letras maiúsculas!");

        RuleFor(x => x.Para).NotEmpty().WithMessage("Moeda de destino é obrigatória!")
            .Length(3).WithMessage("Código da moeda deve ter 3 caracteres!")
            .Matches("^[A-Z]{3}$").WithMessage("Código deve ser em letras maiúsculas!");

        RuleFor(x => x.Valor).GreaterThan(0).WithMessage("Valor deve ser maior que zero!")
            .LessThanOrEqualTo(decimal.MaxValue).WithMessage("Valor muito grande!");

        RuleFor(x => x).Custom((request, context) =>
        {
            if (request.De == request.Para)
                context.AddFailure("De e Para não podem ser a mesma moeda!");
        });
    }
}
