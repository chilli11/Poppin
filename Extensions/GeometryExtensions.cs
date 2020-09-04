using NetTopologySuite.Geometries;
using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Extensions
{
    public class LocDistance
				{
        public LocDistance() { }
        public LocDistance(string locId, int d)
								{
            LocationId = locId;
            DistanceM = d;
								}

        public string LocationId { get; set; }
        public int DistanceM { get; set; }
				}

    static class GeometryExtensions
    {
        static readonly CoordinateSystemServices _coordinateSystemServices
            = new CoordinateSystemServices(
                new CoordinateSystemFactory(),
                new CoordinateTransformationFactory(),
                new Dictionary<int, string>
                {
                    // Coordinate systems:
                    [4326] = GeographicCoordinateSystem.WGS84.WKT
                });

        public static Geometry ProjectTo(this Geometry geometry, int srid)
        {
            var transformation = _coordinateSystemServices.CreateTransformation(geometry.SRID, srid);

            var result = geometry.Copy();
            result.Apply(new MathTransformFilter(transformation.MathTransform));

            return result;
        }

        class MathTransformFilter : ICoordinateSequenceFilter
        {
            readonly MathTransform _transform;

            public MathTransformFilter(MathTransform transform)
                => _transform = transform;

            public bool Done => false;
            public bool GeometryChanged => true;

            public void Filter(CoordinateSequence seq, int i)
            {
                var result = _transform.Transform(
                    new[]
                    {
                    seq.GetOrdinate(i, Ordinate.X),
                    seq.GetOrdinate(i, Ordinate.Y)
                    });
                seq.SetOrdinate(i, Ordinate.X, result[0]);
                seq.SetOrdinate(i, Ordinate.Y, result[1]);
            }
        }
    }
}
