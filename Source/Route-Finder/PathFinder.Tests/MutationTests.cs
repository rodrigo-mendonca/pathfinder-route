﻿using CalcRoute.GeneticAlgorithm;
using CalcRoute.GeneticAlgorithm.Abstraction;
using CalcRoute.GeneticAlgorithm.Mutation;
using CalcRoute.Routes;
using FakeItEasy;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PathFinder.Tests
{
    public class MutationTests
    {

        [Fact]
        public void SwapMutation()
        {
            var settings = new GASettings();
            var random = A.Fake<IRandom>();
            var map = A.Dummy<Roteiro>();
            var locals = GetLocals(4);

            const int
                indexTruck1 = 0,
                indexTruck2 = 1,
                indexLocal1 = 0,
                indexLocal2 = 1;

            A.CallTo(() => random.Next(A<int>._, A<int>._)).ReturnsNextFromSequence(indexTruck1, indexTruck2, indexLocal1, indexLocal2);

            var gen = new Genome(map, settings)
            {
                Trucks = new[] {
                    new Truck {
                        Locals = new[]{ locals[0], locals[1] }
                    },
                    new Truck {
                        Locals = new[]{ locals[2], locals[3] }
                    }

                }
            };

            var mutate = new SwapMutation(settings, random);
            var newGen = mutate.Apply(gen);

            newGen.Trucks[indexTruck1].Locals[indexLocal1]
                .Should().BeEquivalentTo(
                    gen.Trucks[indexTruck2].Locals[indexLocal2],
                    o => o.WithStrictOrdering());

            newGen.Trucks[indexTruck2].Locals[indexLocal2]
                .Should().BeEquivalentTo(
                    gen.Trucks[indexTruck1].Locals[indexLocal1],
                    o => o.WithStrictOrdering());
        }

        [Fact]
        public void InversionMutation()
        {
            var settings = new GASettings();
            var random = A.Fake<IRandom>();
            var map = A.Dummy<Roteiro>();
            var locals = GetLocals(5);

            const int
                indexTruck = 0,
                indexLocalStart = 1,
                length = 3;

            A.CallTo(() => random.Next(A<int>._, A<int>._))
                .ReturnsNextFromSequence(
                    indexTruck,
                    indexLocalStart,
                    length
                );

            var gen = new Genome(map, settings)
            {
                Trucks = new[] {
                    new Truck {
                        Locals = locals
                    }
                }
            };

            var mutate = new InversionMutation(settings, random);
            var newGen = mutate.Apply(gen);

            var genLocals = newGen.Trucks[indexTruck].Locals;

            var current = genLocals.Skip(indexLocalStart).Take(length).ToArray();
            var expect = locals.Skip(indexLocalStart).Take(length).Reverse().ToArray();


            current.Should().BeEquivalentTo(expect, o => o.WithStrictOrdering());

        }

        [Fact]
        public void InsertionMutationShouldCreateANewRoute()
        {
            var settings = new GASettings();
            var random = A.Fake<IRandom>();
            var map = A.Dummy<Roteiro>();
            var locals = GetLocals(6);

            const int
                indexTruck = 0,
                indexLocalFrom = 1,
                indexLocalTo = 0;

            A.CallTo(() => random.NextDouble())
                .ReturnsNextFromSequence(0, 0);

            A.CallTo(() => random.Next(A<int>._, A<int>._))
                .ReturnsNextFromSequence(
                    indexTruck,
                    indexLocalFrom,
                    indexLocalTo
                );

            var gen = new Genome(map, settings)
            {
                Trucks = new[] {
                    new Truck {
                        Locals = locals.Take(3).ToArray()
                    },
                    new Truck {
                        Locals = locals.Skip(3).ToArray()
                    },
                    new Truck {
                        Locals = Enumerable.Empty<Local>().ToArray()
                    }


                }
            };

            var mutate = new InsertionMutation(settings, random);
            var newGen = mutate.Apply(gen);

            var expectLocals = newGen.Trucks[2].Locals;

            A.CallTo(() => random.Next(0, 0)).MustHaveHappenedOnceExactly();

            newGen.Trucks[0].Locals.Should().HaveCount(2);
            newGen.Trucks[0].Locals.Should().NotContain(locals[1]);


            newGen.Trucks[1].Locals.Should().HaveCount(3);

            expectLocals.Should().ContainSingle();
            expectLocals.Should().Contain(locals[1]);

        }


        [Fact]
        public void InsertionMutationShouldNotCreateANewRoute()
        {
            var settings = new GASettings();
            var random = A.Fake<IRandom>();
            var map = A.Dummy<Roteiro>();
            var locals = GetLocals(6);

            const int
                indexTruck = 0,
                indexTruckDestination = 1,
                indexLocalFrom = 1,
                indexLocalTo = 2;

            A.CallTo(() => random.NextDouble())
                .ReturnsNextFromSequence(0, 1);

            A.CallTo(() => random.Next(A<int>._, A<int>._))
                .ReturnsNextFromSequence(
                    indexTruck,
                    indexTruckDestination,
                    indexLocalFrom,
                    indexLocalTo
                );

            var gen = new Genome(map, settings)
            {
                Trucks = new[] {
                    new Truck {
                        Locals = locals.Take(3).ToArray()
                    },
                    new Truck {
                        Locals = locals.Skip(3).ToArray()
                    },
                    new Truck {
                        Locals = Enumerable.Empty<Local>().ToArray()
                    }


                }
            };

            var mutate = new InsertionMutation(settings, random);
            var newGen = mutate.Apply(gen);

            newGen.Trucks[indexTruck].Locals.Should().HaveCount(2);
            newGen.Trucks[indexTruckDestination].Locals.Should().HaveCount(4);
            newGen.Trucks[indexTruckDestination].Locals[indexLocalTo].Should().Be(locals[1]);
            newGen.Trucks[2].Locals.Should().BeEmpty();

        }

        [Fact]
        public void DisplacementMutationShouldCreateANewRoute()
        {

            var settings = new GASettings();
            var random = A.Fake<IRandom>();
            var map = A.Dummy<Roteiro>();
            var locals = GetLocals(8);

            const int
                indexTruck = 0,
                indexLocalFrom = 1,
                length = 2,
                indexLocalTo = 0;

            A.CallTo(() => random.NextDouble())
                .ReturnsNextFromSequence(0, 0);

            A.CallTo(() => random.Next(A<int>._, A<int>._))
                .ReturnsNextFromSequence(
                    indexTruck,
                    indexLocalFrom,
                    length,
                    indexLocalTo
                );

            var gen = new Genome(map, settings)
            {
                Trucks = new[] {
                    new Truck {
                        Locals = locals.Take(4).ToArray()
                    },
                    new Truck {
                        Locals = locals.Skip(4).ToArray()
                    },
                    new Truck {
                        Locals = Enumerable.Empty<Local>().ToArray()
                    }


                }
            };

            var mutate = new DisplacementMutation(settings, random);
            var newGen = mutate.Apply(gen);

            var expectLocals = newGen.Trucks[2].Locals;
            var changedLocations = new[] { locals[1], locals[2] };

            A.CallTo(() => random.Next(0, 0)).MustHaveHappenedOnceExactly();

            newGen.Trucks[0].Locals.Should().HaveCount(2);
            changedLocations.Should().NotBeSubsetOf(newGen.Trucks[0].Locals);

            newGen.Trucks[1].Locals.Should().HaveCount(4);

            expectLocals.Should().HaveCount(2);
            expectLocals.Should().BeEquivalentTo(changedLocations,
                                    option => option.WithStrictOrdering()
                                 );

        }



        [Fact]
        public void DisplacemenMutationShouldNotCreateANewRoute()
        {

            var settings = new GASettings();
            var random = A.Fake<IRandom>();
            var map = A.Dummy<Roteiro>();
            var locals = GetLocals(8);

            const int
                indexTruck = 1,
                indexTruckDestination = 0,
                indexLocalFrom = 1,
                length = 2,
                indexLocalTo = 1;

            A.CallTo(() => random.NextDouble())
                .ReturnsNextFromSequence(0, 1);

            A.CallTo(() => random.Next(A<int>._, A<int>._))
                .ReturnsNextFromSequence(
                    indexTruck,
                    indexTruckDestination,
                    indexLocalFrom,
                    length,
                    indexLocalTo
                );

            var gen = new Genome(map, settings)
            {
                Trucks = new[] {
                    new Truck {
                        Locals = locals.Take(4).ToArray()
                    },
                    new Truck {
                        Locals = locals.Skip(4).ToArray()
                    },
                    new Truck {
                        Locals = Enumerable.Empty<Local>().ToArray()
                    }


                }
            };

            var mutate = new DisplacementMutation(settings, random);
            var newGen = mutate.Apply(gen);


            var changedLocations = new[] { locals[5], locals[6] };

            newGen.Trucks[indexTruck].Locals.Should().HaveCount(2);
            changedLocations.Should().NotBeSubsetOf(newGen.Trucks[indexTruck].Locals);

            newGen.Trucks[indexTruckDestination].Locals.Should().HaveCount(6);
            changedLocations.Should().BeSubsetOf(newGen.Trucks[indexTruckDestination].Locals);

            newGen.Trucks[indexTruckDestination].Locals[1].Should().Be(changedLocations[0]);
            newGen.Trucks[indexTruckDestination].Locals[2].Should().Be(changedLocations[1]);

            newGen.Trucks[2].Locals.Should().BeEmpty();


        }

        static IList<Local> GetLocals(int qtd) =>
             Enumerable.Range(0, qtd)
             .Select(e => new Local(e.ToString()) { Latitude = e, Longitude = e })
             .ToArray();


    }
}
