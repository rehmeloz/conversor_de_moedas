namespace ConversorMoedas.Validators;
using FluentValidation;
using ConversorMoedas.Models;
using ConversorMoedas.Data;
using Microsoft.EntityFrameworkCore;

public class ConversaoValidator : AbstractValidator<ConversaoRequest>
{
    private readonly AppDbContext _context;

    public ConversaoValidator(AppDbContext context)
    {
        _context = context;

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

        RuleFor(x => x).CustomAsync(ValidarMoedasNoBD);
    }

    private async Task ValidarMoedasNoBD(ConversaoRequest request, ValidationContext<ConversaoRequest> context, CancellationToken cancellationToken)
    {
        var moedaDeExiste = await _context.Moedas.AnyAsync(m => m.Codigo == request.De, cancellationToken);

        var moedaParaExiste = await _context.Moedas.AnyAsync(m => m.Codigo == request.Para, cancellationToken);

        if (!moedaDeExiste)
            context.AddFailure("De", $"Moeda '{request.De}' não encontrada");

        if (!moedaParaExiste)
            context.AddFailure("Para", $"Moeda '{request.Para}' não encontrada");
    }
}
