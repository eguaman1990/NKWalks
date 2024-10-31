using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext dbContext;

        public SQLWalkRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Walk> CreateAsync(Walk walk)
        {
            await dbContext.Walks.AddAsync( walk );
            await dbContext.SaveChangesAsync( );

            return walk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var existingWalk = await dbContext.Walks.FirstOrDefaultAsync( w => w.Id == id );
            if(existingWalk == null)
            {
                return null;
            }
            dbContext.Walks.Remove( existingWalk );
            await dbContext.SaveChangesAsync( );
            return existingWalk;
        }

        public async Task<List<Walk>> GetAllAsync(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var walks = dbContext.Walks.Include( "Difficulty" ).Include( "Region" ).AsQueryable( );

            //Filtering
            if(string.IsNullOrWhiteSpace( filterOn ) == false && string.IsNullOrWhiteSpace( filterQuery ) == false)
            {
                switch(filterOn.ToLower( ))
                {
                    case "name":
                        walks = walks.Where( w => w.Name.Contains( filterQuery ) );
                        break;
                        //case "description":
                        //    walks = walks.Where( w => w.Description.Contains( filterQuery ) );
                        //    break;
                        //case "region":
                        //    walks = walks.Where( w => w.Region.Name.Contains( filterQuery ) );
                        //    break;
                        //case "difficulty":
                        //    walks = walks.Where( w => w.Difficulty.Name.Contains( filterQuery ) );
                        //    break;
                }
            }

            //Sorting
            if(string.IsNullOrWhiteSpace( sortBy ) == false)
            {
                if(sortBy.Equals( "Name", StringComparison.OrdinalIgnoreCase ))
                {
                    walks = !isAscending ? walks.OrderByDescending( w => w.Name ) : walks.OrderBy( w => w.Name );
                }
                else if(sortBy.Equals( "LengthInKm", StringComparison.OrdinalIgnoreCase ))
                {
                    walks = !isAscending ? walks.OrderByDescending( w => w.LengthInKm ) : walks.OrderBy( w => w.LengthInKm );
                }
            }

            //Pagination 
            var skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip( skipResults ).Take( pageSize ).ToListAsync( );
            //return await dbContext.Walks.Include("Difficulty").Include("Region").ToListAsync();
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await dbContext.Walks.Include( "Difficulty" ).Include( "Region" ).FirstOrDefaultAsync( w => w.Id == id );
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk updatedWalk)
        {
            var existingWalk = await dbContext.Walks.FirstOrDefaultAsync( x => x.Id == id );
            if(existingWalk == null)
            {
                return null;
            }
            existingWalk.Name = updatedWalk.Name;
            existingWalk.Description = updatedWalk.Description;
            existingWalk.LengthInKm = updatedWalk.LengthInKm;
            existingWalk.RegionId = updatedWalk.RegionId;
            existingWalk.DifficultyId = updatedWalk.DifficultyId;
            existingWalk.WalkImageUrl = updatedWalk.WalkImageUrl;

            await dbContext.SaveChangesAsync( );
            return existingWalk;

        }
    }
}
