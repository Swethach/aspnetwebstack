﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Web.Http.OData.Builder;
using Microsoft.Data.Edm;
using Microsoft.Data.OData;
using Microsoft.TestCommon;

namespace System.Web.Http.OData.Query.Validators
{
    public class SkipQueryValidatorTest
    {
        private SkipQueryValidator _validator;
        private ODataConventionModelBuilder _builder;
        private IEdmModel _model;
        private ODataQueryContext _context;
           
        public SkipQueryValidatorTest()
        {
            _validator = new SkipQueryValidator();
            _builder = new ODataConventionModelBuilder();
            _builder.EntitySet<QueryCompositionCustomer>("Customer");
            _model = _builder.GetEdmModel();
            _context = new ODataQueryContext(_model, typeof(QueryCompositionCustomer));
        }

        [Fact]
        public void ValidateThrowsOnNullOption()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _validator.Validate(null, new ODataValidationSettings()));
        }

        [Fact]
        public void ValidateThrowsOnNullSettings()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _validator.Validate(new SkipQueryOption("2", _context), null));
        }

        [Fact]
        public void ValidateThrowsWhenLimitIsExceeded()
        {
            ODataValidationSettings settings = new ODataValidationSettings()
            {
                MaxSkip = 10
            };

            Assert.Throws<ODataException>(() =>
                _validator.Validate(new SkipQueryOption("11", _context), settings));
        }

        [Fact]
        public void ValidatePassWhenLimitIsReached()
        {
            ODataValidationSettings settings = new ODataValidationSettings()
            {
                MaxSkip = 10
            };
            
            _validator.Validate(new SkipQueryOption("10", _context), settings);
        }

        [Fact]
        public void ValidatePassWhenLimitIsNotReached()
        {
            ODataValidationSettings settings = new ODataValidationSettings()
            {
                MaxSkip = 10
            };

            _validator.Validate(new SkipQueryOption("9", _context), settings);
        }
    }
}
