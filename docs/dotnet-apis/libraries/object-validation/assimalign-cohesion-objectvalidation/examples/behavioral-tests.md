# Example: Object Validation Behavioral Tests

Exercise validator construction, retained exception errors, indexed collection sources, evaluation order, and member failures using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are complete test-file listings from the package test project. Keep them in that project
when running them: the project supplies its package references, generated sources, and any shared
fixtures. The using blocks make the test-framework import explicit where the original project
supplies it globally.

## Code

### ValidatorFactoryBuilderTests.cs

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Xunit;

namespace Assimalign.Cohesion.ObjectValidation.Tests;

public class ValidatorFactoryBuilderTests
{
    [Fact]
    public void TestFactoryBuilder()
    {
        var factory = ValidatorFactoryBuilder.Create(builder =>
        {
            builder.AddValidator("default", builder =>
            {
                builder.AddProfile<ProfileBuilderTest>();
            });
        });


        var validator = factory.CreateValidator("default");

        var result = validator.Validate(new Test());

        Assert.False(result.IsValid);
    }

    private partial class Test
    {
        public string FirstName { get; set; }
    }
    public class ProfileBuilderTest : ValidationProfileBuilder
    {
        protected override void OnBuild(IValidationProfileBuilder builder)
        {
            builder.CreateProfile<Test>(descriptor =>
            {
                descriptor.RuleFor(p => p.FirstName)
                    .NotNull()
                    .NotEmpty();
            });
        }
    }
}
```

### ValidationFailureExceptionTests.cs

```csharp
using System.Collections.Generic;
using System.Linq;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.ObjectValidation.Tests;

public sealed class ValidationFailureExceptionTests
{
    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Constructor: Should retain context errors")]
    public void Constructor_WithContext_ShouldRetainErrors()
    {
        // Arrange
        var context = new ValidationContext<object>(new object());
        context.AddFailure("Name", "Name is required.");
        context.AddFailure("Age", "Age is out of range.");
        var errors = context.Errors.ToArray();

        // Act
        var exception = new ValidationFailureException(context);
        context.AddFailure("Later", "Added after construction.");

        // Assert
        exception.Errors.Count.ShouldBe(2);
        exception.Errors[0].ShouldBeSameAs(errors[0]);
        exception.Errors[1].ShouldBeSameAs(errors[1]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Constructor: Should retain enumerable errors")]
    public void Constructor_WithEnumerable_ShouldRetainErrors()
    {
        // Arrange
        var first = new ValidationError { Code = "required", Source = "Name", Message = "Name is required." };
        var second = new ValidationError { Code = "range", Source = "Age", Message = "Age is out of range." };
        var errors = new List<IValidationError> { first, second };

        // Act
        var exception = new ValidationFailureException(errors);
        errors.Clear();

        // Assert
        exception.Errors.Count.ShouldBe(2);
        exception.Errors[0].ShouldBeSameAs(first);
        exception.Errors[1].ShouldBeSameAs(second);
        ((ICollection<IValidationError>)exception.Errors).IsReadOnly.ShouldBeTrue();
    }
}
```

### RuleCollectionErrorSourceTests.cs

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.ObjectValidation.Tests;

public class RuleCollectionErrorSourceTests
{
    [Theory(DisplayName = "Cohesion Test [ObjectValidation] - RuleForEach: scalar sources include each element's index on every rule and validation")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Validate_ScalarCollectionFailures_ShouldIndexEachElementWithoutChangingSharedRules(bool asynchronous)
    {
        // Arrange
        IValidator validator = Validator.Create(builder =>
        {
            builder.AddOptions(options => options.ContinueThroughValidationChain = true);
            builder.AddProfile(new ScalarProfile());
        });
        var model = new CollectionModel { Names = ["valid", "", "valid", ""] };

        // Act
        ValidationResult first = asynchronous
            ? await validator.ValidateAsync(model, CancellationToken.None)
            : validator.Validate(model);
        ValidationResult second = asynchronous
            ? await validator.ValidateAsync(new CollectionModel { Names = [""] }, CancellationToken.None)
            : validator.Validate(new CollectionModel { Names = [""] });

        // Assert
        first.Errors.Select(error => error.Source).ShouldBe([
            "model => model.Names[1]", "model => model.Names[3]",
            "model => model.Names[1]", "model => model.Names[3]"
        ]);
        second.Errors.Select(error => error.Source).ShouldBe([
            "model => model.Names[0]", "model => model.Names[0]"
        ]);
        first.Errors.All(error => error.Code == "400" && !string.IsNullOrEmpty(error.Message)).ShouldBeTrue();
    }

    [Theory(DisplayName = "Cohesion Test [ObjectValidation] - RuleForEach: nested ChildRules and UseProfile sources include every collection index")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Validate_NestedCollections_ShouldComposeAllElementIndices(bool asynchronous)
    {
        // Arrange
        IValidator validator = Validator.Create(builder => builder.AddProfile(new NestedProfile()));
        var model = new CollectionModel
        {
            Groups = [
                new Group { Items = [new Item { Quantity = 1 }] },
                new Group { Items = [new Item { Quantity = 2 }, new Item { Quantity = 3 }, new Item()] },
                new Group { Items = [new Item { Quantity = 1 }, new Item()] }
            ]
        };

        // Act
        ValidationResult result = asynchronous
            ? await validator.ValidateAsync(model, CancellationToken.None)
            : validator.Validate(model);

        // Assert
        result.Errors.Select(error => error.Source).ShouldBe([
            "model => model.Groups[1].Items[2].Quantity",
            "model => model.Groups[2].Items[1].Quantity"
        ]);
    }

    [Theory(DisplayName = "Cohesion Test [ObjectValidation] - RuleForEach: explicit sources remain unchanged for scalar and nested rules")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Validate_ExplicitCollectionSources_ShouldKeepThemAsWritten(bool asynchronous)
    {
        // Arrange
        IValidator validator = Validator.Create(builder => builder.AddProfile(new ExplicitSourceProfile()));
        var model = new CollectionModel
        {
            Names = ["", ""],
            Groups = [new Group { Items = [new Item()] }]
        };

        // Act
        ValidationResult result = asynchronous
            ? await validator.ValidateAsync(model, CancellationToken.None)
            : validator.Validate(model);

        // Assert
        result.Errors.Select(error => error.Source).ShouldBe(["name", "name", "quantity"]);
    }

    public sealed class CollectionModel
    {
        public IEnumerable<string> Names { get; set; } = [];

        public IEnumerable<Group> Groups { get; set; } = [];
    }

    public sealed class Group
    {
        public IEnumerable<Item> Items { get; set; } = [];
    }

    public sealed class Item
    {
        public int Quantity { get; set; }
    }

    public sealed class ScalarProfile : ValidationProfile<CollectionModel>
    {
        public override void Configure(IValidationRuleDescriptor<CollectionModel> descriptor)
        {
            descriptor.RuleForEach(model => model.Names).NotEmpty().Matches("^valid$");
        }
    }

    public sealed class NestedProfile : ValidationProfile<CollectionModel>
    {
        public override void Configure(IValidationRuleDescriptor<CollectionModel> descriptor)
        {
            descriptor.RuleForEach(model => model.Groups).ChildRules(group =>
                group.RuleForEach(value => value.Items).UseProfile(new ItemProfile()));
        }
    }

    public sealed class ItemProfile : ValidationProfile<Item>
    {
        public override void Configure(IValidationRuleDescriptor<Item> descriptor)
        {
            descriptor.RuleFor(item => item.Quantity).GreaterThan(0);
        }
    }

    public sealed class ExplicitSourceProfile : ValidationProfile<CollectionModel>
    {
        public override void Configure(IValidationRuleDescriptor<CollectionModel> descriptor)
        {
            descriptor.RuleForEach(model => model.Names).NotEmpty(error => error.Source = "name");
            descriptor.RuleForEach(model => model.Groups).ChildRules(group =>
                group.RuleForEach(value => value.Items).ChildRules(item =>
                    item.RuleFor(value => value.Quantity).GreaterThan(0, error => error.Source = "quantity")));
        }
    }
}
```

### ValidatorEvaluationOrderTests.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.ObjectValidation.Tests;

/// <summary>
/// The order a validator evaluates in (#1221): a profile's members in the order they are declared, and each
/// member's rules in the order they are chained, for single members, collections (<c>RuleForEach</c>), nested
/// profiles (<c>ChildRules</c>, <c>UseProfile</c>) and <c>When</c> blocks. The order decides what is reported:
/// with the default options a chain reports the first of its rules that fails, and
/// <see cref="ValidationMode.Stop"/> stops at the first failing member. Errors are listed in the order they
/// were reported.
/// </summary>
public class ValidatorEvaluationOrderTests
{
    private const string required = "required";
    private const string tooShort = "too short";

    public sealed class Address
    {
        public string? City { get; set; }

        public string? Zip { get; set; }
    }

    public sealed class Parcel
    {
        public string? Label { get; set; }

        public string? Sender { get; set; }

        public string? Recipient { get; set; }

        public string? Reference { get; set; }

        public Address? Destination { get; set; }

        public IEnumerable<string>? Tags { get; set; }

        public IEnumerable<Address>? Stops { get; set; }
    }

    /// <summary>A city, then a zip.</summary>
    public sealed class AddressProfile : ValidationProfile<Address>
    {
        public override void Configure(IValidationRuleDescriptor<Address> descriptor)
        {
            descriptor.RuleFor(address => address.City!).NotEmpty();
            descriptor.RuleFor(address => address.Zip!).NotEmpty();
        }
    }

    /// <summary>The rules each test declares.</summary>
    public sealed class ParcelProfile : ValidationProfile<Parcel>
    {
        private readonly Action<IValidationRuleDescriptor<Parcel>> _configure;

        public ParcelProfile(Action<IValidationRuleDescriptor<Parcel>> configure)
        {
            _configure = configure;
        }

        public override void Configure(IValidationRuleDescriptor<Parcel> descriptor) => _configure(descriptor);
    }

    private static IValidator CreateValidator(Action<IValidationRuleDescriptor<Parcel>> configure, Action<ValidationOptions>? options = null) => Validator.Create(builder =>
    {
        if (options is not null)
        {
            builder.AddOptions(options);
        }

        builder.AddProfile(new ParcelProfile(configure));
    });

    private static string[] Messages(ValidationResult result) => result.Errors.Select(error => error.Message).ToArray();

    private static string[] Sources(ValidationResult result) => result.Errors.Select(error => error.Source).ToArray();

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Order: with the default options a chain reports the first of its rules that fails")]
    public void Validate_DefaultOptionsChainWithSeveralFailingRules_ShouldReportTheFirstChainedRule()
    {
        // Arrange — an empty label fails both of its rules.
        IValidator validator = CreateValidator(descriptor => descriptor.RuleFor(parcel => parcel.Label!)
            .NotEmpty(error => error.Message = required)
            .MinLength(3, error => error.Message = tooShort));

        // Act
        ValidationResult result = validator.Validate(new Parcel { Label = string.Empty });

        // Assert
        Messages(result).ShouldBe([required]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Order: ContinueThroughValidationChain reports a chain's failing rules in the order they are chained")]
    public void Validate_ContinueThroughValidationChain_ShouldReportRulesInChainOrder()
    {
        // Arrange — an empty label fails both of its rules.
        IValidator validator = CreateValidator(
            descriptor => descriptor.RuleFor(parcel => parcel.Label!)
                .NotEmpty(error => error.Message = required)
                .MinLength(3, error => error.Message = tooShort),
            options => options.ContinueThroughValidationChain = true);

        // Act
        ValidationResult result = validator.Validate(new Parcel { Label = string.Empty });

        // Assert
        Messages(result).ShouldBe([required, tooShort]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Order: Stop inside a nested profile reports its first declared failing member")]
    public void Validate_StopModeNestedProfileWithSeveralFailingMembers_ShouldReportTheFirstDeclaredMember()
    {
        // Arrange — both of the destination's members fail.
        IValidator validator = CreateValidator(
            descriptor => descriptor.RuleFor(parcel => parcel.Destination!).ChildRules(destination =>
            {
                destination.RuleFor(address => address.City!).NotEmpty();
                destination.RuleFor(address => address.Zip!).NotEmpty();
            }),
            options => options.ValidationMode = ValidationMode.Stop);

        // Act
        ValidationResult result = validator.Validate(new Parcel { Destination = new Address() });

        // Assert
        Sources(result).ShouldBe(["parcel => parcel.Destination.City"]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Order: a collection's chain stops after its first rule, which reports the elements in sequence")]
    public void Validate_DefaultOptionsCollectionWithSeveralFailingRules_ShouldReportTheFirstChainedRuleForEveryElement()
    {
        // Arrange — both rules fail every element.
        IValidator validator = CreateValidator(descriptor => descriptor.RuleForEach(parcel => parcel.Tags!)
            .Custom((tag, context) => context.AddFailure("tags", $"first rule: {tag}"))
            .Custom((tag, context) => context.AddFailure("tags", $"second rule: {tag}")));

        // Act
        ValidationResult result = validator.Validate(new Parcel { Tags = ["a", "b"] });

        // Assert
        Messages(result).ShouldBe(["first rule: a", "first rule: b"]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Order: ContinueThroughValidationChain runs a collection's rules in chain order, each over the elements in sequence")]
    public void Validate_ContinueThroughValidationChainCollection_ShouldReportRulesInChainOrderAndElementsInSequence()
    {
        // Arrange — both rules fail every element.
        IValidator validator = CreateValidator(
            descriptor => descriptor.RuleForEach(parcel => parcel.Tags!)
                .Custom((tag, context) => context.AddFailure("tags", $"first rule: {tag}"))
                .Custom((tag, context) => context.AddFailure("tags", $"second rule: {tag}")),
            options => options.ContinueThroughValidationChain = true);

        // Act
        ValidationResult result = validator.Validate(new Parcel { Tags = ["a", "b"] });

        // Assert
        Messages(result).ShouldBe(["first rule: a", "first rule: b", "second rule: a", "second rule: b"]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Order: a collection of nested profiles reports element by element, each element's members in declaration order")]
    public void Validate_CollectionOfNestedProfiles_ShouldReportElementsInSequenceAndMembersInDeclarationOrder()
    {
        // Arrange — the first stop has no city; the second has neither a city nor a zip.
        IValidator validator = CreateValidator(descriptor => descriptor.RuleForEach(parcel => parcel.Stops!).UseProfile(new AddressProfile()));

        // Act
        ValidationResult result = validator.Validate(new Parcel { Stops = [new Address { Zip = "N1" }, new Address()] });

        // Assert
        Sources(result).ShouldBe(["parcel => parcel.Stops[0].City", "parcel => parcel.Stops[1].City", "parcel => parcel.Stops[1].Zip"]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Order: a When block's members are evaluated where the block is declared")]
    public void Validate_WhenBlockBetweenMembers_ShouldEvaluateItsMembersInPlace()
    {
        // Arrange — every member fails; the sender and the recipient are declared inside the block.
        IValidator validator = CreateValidator(descriptor =>
        {
            descriptor.RuleFor(parcel => parcel.Label!).NotEmpty();
            descriptor.When(parcel => parcel.Destination is null, unaddressed =>
            {
                unaddressed.RuleFor(parcel => parcel.Sender!).NotEmpty();
                unaddressed.RuleFor(parcel => parcel.Recipient!).NotEmpty();
            });
            descriptor.RuleFor(parcel => parcel.Reference!).NotEmpty();
        });

        // Act
        ValidationResult result = validator.Validate(new Parcel());

        // Assert
        Sources(result).ShouldBe(
        [
            "parcel => parcel.Label",
            "parcel => parcel.Sender",
            "parcel => parcel.Recipient",
            "parcel => parcel.Reference"
        ]);
    }
}
```

### ValidatorMemberFailureTests.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.ObjectValidation.Tests;

/// <summary>
/// Which failures a validator reports under each option. <see cref="ValidationMode"/> decides whether every
/// failing member is reported (<see cref="ValidationMode.Cascade"/>, the default) or validation stops at the
/// first failing member (<see cref="ValidationMode.Stop"/>).
/// <see cref="ValidationOptions.ContinueThroughValidationChain"/> decides whether a failing rule stops the rest
/// of its own member's chain (the default) or every rule runs. A member's chain never stops because another
/// member failed.
/// </summary>
/// <remarks>
/// The assertions pin order: members run in the order they are declared and chained rules in the order they
/// are chained, and errors are listed in the order they were reported (#1221).
/// </remarks>
public class ValidatorMemberFailureTests
{
    private const string nameSource = "customer => customer.Name";
    private const string ageSource = "customer => customer.Age";
    private const string citySource = "customer => customer.Address.City";
    private const string zipSource = "customer => customer.Address.Zip";
    private const string tagsSource = "customer => customer.Tags";

    public sealed class Address
    {
        public string? City { get; set; }

        public string? Zip { get; set; }
    }

    public sealed class Customer
    {
        public string? Name { get; set; }

        public int Age { get; set; }

        public Address? Address { get; set; }

        public IEnumerable<string>? Tags { get; set; }
    }

    /// <summary>
    /// A name checked by two chained rules, an adult age, a nested address with two members, and tags checked
    /// by two chained rules per element.
    /// </summary>
    public sealed class CustomerProfile : ValidationProfile<Customer>
    {
        public override void Configure(IValidationRuleDescriptor<Customer> descriptor)
        {
            descriptor.RuleFor(customer => customer.Name!).NotEmpty().MinLength(3);
            descriptor.RuleFor(customer => customer.Age).GreaterThanOrEqualTo(18);
            descriptor.RuleFor(customer => customer.Address!).ChildRules(address =>
            {
                address.RuleFor(a => a.City!).NotEmpty();
                address.RuleFor(a => a.Zip!).NotEmpty();
            });
            descriptor.RuleForEach(customer => customer.Tags!).NotEmpty().MinLength(3);
        }
    }

    private static IValidator CreateValidator(Action<ValidationOptions>? configure = null) => Validator.Create(builder =>
    {
        if (configure is not null)
        {
            builder.AddOptions(configure);
        }

        builder.AddProfile(new CustomerProfile());
    });

    // Valid in every member: each test breaks the members it is about.
    private static Customer CreateCustomer() => new()
    {
        Name = "Ada",
        Age = 36,
        Address = new Address { City = "London", Zip = "N1" },
        Tags = ["vip"]
    };

    private static string[] Sources(ValidationResult result) => result.Errors.Select(error => error.Source).ToArray();

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Default options: every failing member is reported, nested members included")]
    public void Validate_DefaultOptionsSeveralFailingMembers_ShouldReportEveryMember()
    {
        // Arrange — the name fails one of its two rules; the age and both nested members fail.
        Customer customer = CreateCustomer();
        customer.Name = "Al";
        customer.Age = 12;
        customer.Address = new Address();

        // Act
        ValidationResult result = CreateValidator().Validate(customer);

        // Assert
        Sources(result).ShouldBe([nameSource, ageSource, citySource, zipSource]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Default options: a member's chain stops at its first failing rule, and the other members still run")]
    public void Validate_DefaultOptionsMemberWithSeveralFailingRules_ShouldReportOneErrorForThatMember()
    {
        // Arrange — an empty name fails both of its rules; the age fails too.
        Customer customer = CreateCustomer();
        customer.Name = string.Empty;
        customer.Age = 12;

        // Act
        ValidationResult result = CreateValidator().Validate(customer);

        // Assert — one error for the name, whose chain stopped, and the age's.
        Sources(result).ShouldBe([nameSource, ageSource]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - ContinueThroughValidationChain: every failing rule of every member is reported")]
    public void Validate_ContinueThroughValidationChain_ShouldReportEveryFailingRuleOfEveryMember()
    {
        // Arrange — an empty name fails both of its rules; the age fails too.
        Customer customer = CreateCustomer();
        customer.Name = string.Empty;
        customer.Age = 12;

        // Act
        ValidationResult result = CreateValidator(options => options.ContinueThroughValidationChain = true).Validate(customer);

        // Assert — both of the name's rules, and the age's.
        Sources(result).ShouldBe([nameSource, nameSource, ageSource]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Stop mode: validation stops at the first failing member")]
    public void Validate_StopModeSeveralFailingMembers_ShouldReportOneMember()
    {
        // Arrange — three members fail, each in one rule.
        Customer customer = CreateCustomer();
        customer.Name = "Al";
        customer.Age = 12;
        customer.Address = new Address { City = string.Empty, Zip = "N1" };

        // Act
        ValidationResult result = CreateValidator(options => options.ValidationMode = ValidationMode.Stop).Validate(customer);

        // Assert — the name, the first failing member in declaration order.
        Sources(result).ShouldBe([nameSource]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Default options: a failing collection member does not stop another member")]
    public void Validate_DefaultOptionsFailingCollectionAndMember_ShouldReportBoth()
    {
        // Arrange — one tag is too short; the age fails too.
        Customer customer = CreateCustomer();
        customer.Tags = ["vip", "x"];
        customer.Age = 12;

        // Act
        ValidationResult result = CreateValidator().Validate(customer);

        // Assert
        Sources(result).ShouldBe([ageSource, tagsSource + "[1]"]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Default options: a collection's chain stops after the rule that fails first, which reports every failing element")]
    public void Validate_DefaultOptionsCollectionWithSeveralFailingRules_ShouldReportOneRulePerFailingElement()
    {
        // Arrange — both tags are empty, so each fails both rules; the age fails too.
        Customer customer = CreateCustomer();
        customer.Tags = [string.Empty, string.Empty];
        customer.Age = 12;

        // Act
        ValidationResult result = CreateValidator().Validate(customer);

        // Assert — the age's error, then one per element from the rule that ran first, none from the rule after it.
        Sources(result).ShouldBe([ageSource, tagsSource + "[0]", tagsSource + "[1]"]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Default options: ValidateAsync reports every failing member")]
    public async Task ValidateAsync_DefaultOptionsSeveralFailingMembers_ShouldReportEveryMember()
    {
        // Arrange
        Customer customer = CreateCustomer();
        customer.Name = "Al";
        customer.Age = 12;

        // Act
        ValidationResult result = await CreateValidator().ValidateAsync(customer, CancellationToken.None);

        // Assert
        Sources(result).ShouldBe([nameSource, ageSource]);
    }

    [Fact(DisplayName = "Cohesion Test [ObjectValidation] - Default options: an error already in the context does not stop any member's rules")]
    public void Validate_ContextWithEarlierError_ShouldRunEveryMembersRules()
    {
        // Arrange — the caller's context already holds an error that no member reported.
        Customer customer = CreateCustomer();
        customer.Name = "Al";
        customer.Age = 12;

        IValidationContext context = new ValidationContext<Customer>(customer);
        context.AddFailure("order", "The order is closed.");

        // Act
        ValidationResult result = CreateValidator().Validate(context);

        // Assert — the caller's error first: errors are listed in the order they were added.
        Sources(result).ShouldBe(["order", nameSource, ageSource]);
    }
}
```

## Walkthrough

### Validator factory builder tests

- **Test entry point** — `TestFactoryBuilder` contains the setup, invocation, and assertions for this case.

### ValidationFailureExceptionTests

- **Test entry point** — `Constructor_WithContext_ShouldRetainErrors` — Constructor: Should retain context errors.
- **Test entry point** — `Constructor_WithEnumerable_ShouldRetainErrors` — Constructor: Should retain enumerable errors.

### RuleCollectionErrorSourceTests

- **Test entry point** — `Validate_ScalarCollectionFailures_ShouldIndexEachElementWithoutChangingSharedRules` — RuleForEach: scalar sources include each element's index on every rule and validation.
- **Test entry point** — `Validate_NestedCollections_ShouldComposeAllElementIndices` — RuleForEach: nested ChildRules and UseProfile sources include every collection index.
- **Test entry point** — `Validate_ExplicitCollectionSources_ShouldKeepThemAsWritten` — RuleForEach: explicit sources remain unchanged for scalar and nested rules.

### ValidatorEvaluationOrderTests

- **Test entry point** — `Validate_DefaultOptionsChainWithSeveralFailingRules_ShouldReportTheFirstChainedRule` — Order: with the default options a chain reports the first of its rules that fails.
- **Test entry point** — `Validate_ContinueThroughValidationChain_ShouldReportRulesInChainOrder` — Order: ContinueThroughValidationChain reports a chain's failing rules in the order they are chained.
- **Test entry point** — `Validate_StopModeNestedProfileWithSeveralFailingMembers_ShouldReportTheFirstDeclaredMember` — Order: Stop inside a nested profile reports its first declared failing member.
- **Test entry point** — `Validate_DefaultOptionsCollectionWithSeveralFailingRules_ShouldReportTheFirstChainedRuleForEveryElement` — Order: a collection's chain stops after its first rule, which reports the elements in sequence.
- **Test entry point** — `Validate_ContinueThroughValidationChainCollection_ShouldReportRulesInChainOrderAndElementsInSequence` — Order: ContinueThroughValidationChain runs a collection's rules in chain order, each over the elements in sequence.
- **Test entry point** — `Validate_CollectionOfNestedProfiles_ShouldReportElementsInSequenceAndMembersInDeclarationOrder` — Order: a collection of nested profiles reports element by element, each element's members in declaration order.
- **Test entry point** — `Validate_WhenBlockBetweenMembers_ShouldEvaluateItsMembersInPlace` — Order: a When block's members are evaluated where the block is declared.

### ValidatorMemberFailureTests

- **Test entry point** — `Validate_DefaultOptionsSeveralFailingMembers_ShouldReportEveryMember` — Default options: every failing member is reported, nested members included.
- **Test entry point** — `Validate_DefaultOptionsMemberWithSeveralFailingRules_ShouldReportOneErrorForThatMember` — Default options: a member's chain stops at its first failing rule, and the other members still run.
- **Test entry point** — `Validate_ContinueThroughValidationChain_ShouldReportEveryFailingRuleOfEveryMember` — ContinueThroughValidationChain: every failing rule of every member is reported.
- **Test entry point** — `Validate_StopModeSeveralFailingMembers_ShouldReportOneMember` — Stop mode: validation stops at the first failing member.
- **Test entry point** — `Validate_DefaultOptionsFailingCollectionAndMember_ShouldReportBoth` — Default options: a failing collection member does not stop another member.
- **Test entry point** — `Validate_DefaultOptionsCollectionWithSeveralFailingRules_ShouldReportOneRulePerFailingElement` — Default options: a collection's chain stops after the rule that fails first, which reports every failing element.
- **Test entry point** — `ValidateAsync_DefaultOptionsSeveralFailingMembers_ShouldReportEveryMember` — Default options: ValidateAsync reports every failing member.
- **Test entry point** — `Validate_ContextWithEarlierError_ShouldRunEveryMembersRules` — Default options: an error already in the context does not stop any member's rules.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/tests/BuilderTests/ValidatorFactoryBuilderTests.cs`.
- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/tests/ValidationFailureExceptionTests.cs`.
- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/tests/RuleTests/RuleCollectionErrorSourceTests.cs`.
- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/tests/ValidatorTests/ValidatorEvaluationOrderTests.cs`.
- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/tests/ValidatorTests/ValidatorMemberFailureTests.cs`.
- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/tests/Assimalign.Cohesion.ObjectValidation.Tests.csproj`.
